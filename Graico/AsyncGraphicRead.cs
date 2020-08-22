using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graico
{
    public class AsyncGraphicRead
    {
        public string FileName { get; set; }
        private List<ZipArchiveEntry> ZipArcEntryList = null;
        private Object thisLock = new Object();
        private string[] graphicFileExt = { ".jpg", ".jpe", ".jpeg", ".gif", ".bmp", ".png", ".tif", ".tiff" };

        /// <summary>
        /// 非同期画像ファイル読み込み
        /// </summary>
        /// <param name="fileName">読み込む画像ファイル名</param>
        /// <param name="progress">進行状況の更新プロバイダー</param>
        /// <returns></returns>
        private async Task<Image>
            GetImageFileAsync(
            string fileName,
            IProgress<int> progress)
        {
            var readList = new List<byte>();
            using (var fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int readsize = Convert.ToInt32(fs.Length / 100);
                if (readsize <= 100)
                {
                    readsize = 100;
                }

                foreach (int i in Enumerable.Range(0, 100))
                {
                    var readBuff = new byte[readsize];
                    var readSize = await fs.ReadAsync(readBuff, 0, (int)readBuff.Length);
                    if (readSize <= 0)
                    {
                        break;
                    }
                    Array.Resize(ref readBuff, readSize);
                    readList.AddRange(readBuff);
                    progress.Report(i + 1);
                    //Debug.WriteLine((i + 1) + "%");
                }
            }
            //var imgconv = new ImageConverter();
            //return (imgconv.ConvertFrom(readList.ToArray()) as Image);
            var imgconv = new ImageConverter();
            var img = imgconv.ConvertFrom(readList.ToArray()) as Image;
            readList.Clear();
            readList = null;
            return (img);
        }

        private async Task GetZipGraphicImage(
            List<Image> imgList,
            int newIndex,
            int DivSize = 4096)
        {
            if (ZipArcEntryList.Count < (newIndex))
            {
                newIndex = 0;
            }
            else if (newIndex < 0)
            {
                newIndex = ZipArcEntryList.Count - 1;
            }
            await Task.Run(() =>
            {
                // ロックをかけないとデータ破壊の恐れあり
                // 付けないとヘッダが壊れているという例外が発生することがある
                lock (thisLock)
                {
                    Debug.WriteLine("ZIP Arc index=" + newIndex);
                    var ZipArc = ZipArcEntryList[newIndex];
                    Debug.WriteLine("Zip new Index File=" + ZipArc.FullName);
                    using (var zipStream = ZipArc.Open())
                    {
                        try
                        {
                            //Image Img = Image.FromStream(zipStream);
                            //Debug.WriteLine("New Index=" + newIndex + " FileName = " + ZipArcEntryList[newIndex].FullName);
                            GetDivideImageFormStream(zipStream, imgList, DivSize);
                            return;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            return;
                        }
                    }
                }
            });
            return;
        }

        /// <summary>
        /// ファイル一覧取得
        /// </summary>
        /// <remarks>
        /// ZIPファイルやディレクトリから、グラフィックファイル一覧を作成する
        /// </remarks>
        /// <param name="fileName">ZIPファイル名/基となる画像ファイル</param>
        public async Task<List<string>> GetFileListAsync(
            string fileName)
        {
            var extens = Path.GetExtension(fileName).ToLower();
            if (extens == ".zip" || extens == ".cbz")
            {
                var zipStream = File.OpenRead(fileName);
                return await Task.Run(() =>
                {
                    try
                    {
                        var arc = new ZipArchive(zipStream, ZipArchiveMode.Read);
                        ZipArcEntryList = arc.Entries
                            .Where(x =>
                            {
                                var ext = Path.GetExtension(x.Name).ToLower();
                                if (graphicFileExt.Contains(ext))
                                {
                                    return true;
                                }
                                return false;
                            })
                            //.OrderBy(x => x.Name, new NaturalComparer())
                            .OrderBy(x => x.Name, new StrNatComparer())
                            .ToList();
                    }
                    catch
                    {
                        return null;
                    }
                    return ZipArcEntryList.Select(x => x.FullName).ToList();
                });

            }
            else
            {
                // 検索ディレクトリ＝カレントディレクトリ
                string currDir = Path.GetDirectoryName(fileName);
                // EnumerateFilesは単一のパターンのみしか指定できないため、
                // とりあえず全部取得し、Whereで絞り込む
                string searchPattern = "*";
                // ファイルリストの取得
                // LINQでファイルの絞り込み(LINQなら複雑な処理を記述しなくて済む)
                // さらに、別途ソートしていたのをLINQでナチュラルソートするように変更
                // List化も別途から一緒にするように変更した
                return await Task.Run(() =>
                {
                    return Directory.EnumerateFiles(currDir, searchPattern)
                    .Where(file =>
                    {
                        // 拡張子の取得
                        string ext = Path.GetExtension(file).ToLower();
                        bool rtc = false;
                        if (graphicFileExt.Contains(ext))
                        {
                            rtc = true;
                        }
                        return rtc;
                    //}).OrderBy(x => x, new NaturalComparer()).ToList();
                    }).OrderBy(x => x, new StrNatComparer()).ToList();
                });
            }
        }

        /// <summary>
        /// ファイルリストから指定のファイルを見つけ出し、
        /// addIndex加算した次のファイル（マイナスなら前）を
        /// 返す
        /// </summary>
        /// <param name="FileList"></param>
        /// <param name="fileName"></param>
        /// <param name="addIndex"></param>
        /// <returns></returns>
        private string GetNextFile(List<string> FileList, string fileName, int addIndex)
        {
            var findIndex = FileList.FindIndex(file => file == fileName);
            if (findIndex < 0)
            {
                return null;
            }
            int index = findIndex + addIndex;
            if (index >= FileList.Count)
            {
                index = 0;
            }
            else if (index < 0)
            {
                index = FileList.Count - 1;
            }
            string getFile = string.Empty;
            while (index < FileList.Count)
            {
                getFile = FileList[index];
                var ext = Path.GetExtension(fileName).ToLower();
                //if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" &&
                //    ext != ".bmp" && ext != ".gif")
                if (!graphicFileExt.Contains(ext))
                {
                    int newIndex = index + addIndex;
                    if (newIndex >= 0 && newIndex < FileList.Count)
                    {
                        index += addIndex;
                    }
                    else if (newIndex < 0)
                    {
                        index = FileList.Count - 1;
                        continue;
                    }
                    else if (newIndex > FileList.Count)
                    {
                        index = 0;
                        continue;
                    }
                }
                else
                {
                    break;
                }
            }
            return getFile;
        }

        private async Task<Image> GetNextImage(
            List<string> FileList,
            string fileName,
            int add,
            bool zipFile)
        {
            var imgList = new List<Image>();
            FileName = GetNextFile(FileList, fileName, add);
            if (zipFile)
            {
                int index = FileList.FindIndex(file => file == FileName);
                await GetZipGraphicImage(imgList, index, 0);
            }
            else
            {
                //return await GetImageFileAsync(FileName, new Progress<int>(prog => { }));
                GetDivideImageFormFile(FileName, imgList, 0);
            }
            return imgList[0];
        }

        public async Task<string> GetIndexImage(
            List<Image> imgList,
            List<string> FileList,
            //string fileName,
            int index,
            bool zipFile,
            int divSize = 4096)
        {
            if (index < 0 || index >= FileList.Count)
            {
                Debug.WriteLine("Index Under/Over Error! index=" + index);
                return null;
            }
            Debug.WriteLine("Index Image=" + index);
            FileName = FileList[index];
            Debug.WriteLine("Get Index Image file=" + FileName);
            if (zipFile)
            {
                await GetZipGraphicImage(imgList, index, divSize);
            }
            else
            {
                //return await GetImageFileAsync(FileName, new Progress<int>(prog => { }));
                GetDivideImageFormFile(FileName, imgList, divSize);
            }
            return FileName;
        }

        private async Task GetNextImage(
            List<Image> imgList,
            List<string> FileList,
            string fileName,
            int add,
            bool zipFile)
        {
            FileName = GetNextFile(FileList, fileName, add);
            Debug.WriteLine("Get Next Image file=" + FileName);
            if (zipFile)
            {
                int index = FileList.FindIndex(file => file == FileName);
                await GetZipGraphicImage(imgList, index);
                return;
            }
            else
            {
                //return await GetImageFileAsync(FileName, new Progress<int>(prog => { }));
                GetDivideImageFormFile(FileName, imgList);
            }
        }

        public static void GetDivideImage(
            Image img,
            List<Image> divImg,
            int DivSize = 4096)
        {
            int height = img.Height;
            // 分割の必要なしなら、読み込んだイメージをリストに追加して返す
            if (height <= DivSize || DivSize <= 0)
            {
                divImg.Add(img);
                return;
            }

            // 分割数計算
            int divNo = 0;
            try
            {
                divNo = height / DivSize + 1;
            }
            catch (DivideByZeroException)
            {
                divNo = 1;
            }
            var enumRange = Enumerable.Range(0, divNo);
            int heightPos = 0;
            // 最後は別途計算する
            int lastSize = height - (DivSize * (divNo - 1));
            // 読み込んだ画像を縦分割する
            enumRange.ForEach(i =>
            {
                int ySize = DivSize;
                if (i == enumRange.Last())
                {
                    ySize = lastSize;
                }
                if (i == enumRange.Last() && lastSize <= 0)
                {
                }
                else
                {
                    //画像ファイルのImageオブジェクトを作成する
                    var image = new Bitmap(img.Width, ySize);
                    //ImageオブジェクトのGraphicsオブジェクトを作成する
                    Graphics g = Graphics.FromImage(image);

                    //切り取る部分の範囲を決定する
                    Rectangle srcRect = new Rectangle(0, heightPos, img.Width, ySize);
                    //描画する部分の範囲を決定する
                    Rectangle desRect = new Rectangle(0, 0, img.Width, srcRect.Height);
                    //画像の一部を描画する
                    g.DrawImage(img, desRect, srcRect, GraphicsUnit.Pixel);

                    //Graphicsオブジェクトのリソースを解放する
                    g.Dispose();
                    heightPos += ySize;
                    divImg.Add(image);
                }
            });
            img.Dispose();
            img = null;
        }

        /// <summary>
        /// 画素数が大きいサイズの画像を縦分割して読み込む
        /// </summary>
        /// <param name="fileName">読み込む画像ファイル名</param>
        /// <param name="divImg">読み込んだ画像イメージリスト</param>
        /// <param name="DivSize">分割する縦サイズ</param>
        public static void GetDivideImageFormFile(
            string fileName,
            List<Image> divImg,
            int DivSize = 4096)
        {
            // まず全部を読み込む(これでメモリ不足になるなら、別の手を考える必要あり)
            var img = Image.FromFile(fileName);
            GetDivideImage(img, divImg, DivSize);
        }

        /// <summary>
        /// 画素数が大きいサイズの画像を縦分割して読み込む
        /// </summary>
        /// <param name="fileStream">読み込む画像ファイル名</param>
        /// <param name="divImg">読み込んだ画像イメージリスト</param>
        /// <param name="DivSize">分割する縦サイズ</param>
        public static void GetDivideImageFormStream(
            Stream fileStream,
            List<Image> divImg,
            int DivSize = 4096)
        {
            // まず全部を読み込む(これでメモリ不足になるなら、別の手を考える必要あり)
            var img = Image.FromStream(fileStream);
            GetDivideImage(img, divImg, DivSize);
        }

        public async Task<Image> GetZoomImage(List<string> fileList, string imageFile, Size newSize, bool zip)
        {
            //描画先とするImageオブジェクトを作成する
            Bitmap canvas = new Bitmap(newSize.Width, newSize.Height);
            //ImageオブジェクトのGraphicsオブジェクトを作成する
            Graphics g = null;
            await Task.Run(() => g = Graphics.FromImage(canvas));

            //画像ファイルを読み込んで、Imageオブジェクトとして取得する
            Image img = await GetNextImage(fileList, imageFile, 0, zip);
            //画像のサイズを2倍にしてcanvasに描画する
            g.DrawImage(img, 0, 0, newSize.Width, newSize.Height);
            //Imageオブジェクトのリソースを解放する
            img.Dispose();

            //Graphicsオブジェクトのリソースを解放する
            g.Dispose();
            return canvas;
        }

        public static void GetImageSize(Stream fs, out Size imgSize)
        {
            using (Image img = Image.FromStream(fs, false, false))
            {
                Console.WriteLine(img.Width + " x " + img.Height);
                imgSize = new Size();
                imgSize.Width = img.Size.Width;
                imgSize.Height = img.Size.Height;
                img.Dispose();
            }
        }

        public void GetZipInImageSize(int index, out Size imgSize)
        {
            var ZipArc = ZipArcEntryList[index];
            using (var zipStream = ZipArc.Open())
            {
                GetImageSize(zipStream, out imgSize);
            }
        }

        public static void GetFileImageSize(string fileName, out Size imgSize)
        {
            using (FileStream fs = File.OpenRead(fileName))
            {
                GetImageSize(fs, out imgSize);
            }
        }

        public void GetPictureSize(List<string> FileList, string fileName, out Size imgSize, bool zip)
        {
            if (zip)
            {
                int index = FileList.FindIndex(file => file == fileName);
                GetZipInImageSize(index, out imgSize);
            }
            else
            {
                GetFileImageSize(fileName, out imgSize);
            }
        }

        public static Image GetZoomImageFromStream(Stream fs, Size imgSize)
        {
            using (Image img = Image.FromStream(fs, false, false))
            {
                //描画先とするImageオブジェクトを作成する
                Bitmap canvas = new Bitmap(imgSize.Width, imgSize.Height);
                //ImageオブジェクトのGraphicsオブジェクトを作成する
                Graphics g = Graphics.FromImage(canvas);
                g.InterpolationMode =
                    InterpolationMode.HighQualityBicubic;
                //画像のサイズを2倍にしてcanvasに描画する
                g.DrawImage(img, 0, 0, imgSize.Width, imgSize.Height);
                //Imageオブジェクトのリソースを解放する
                img.Dispose();

                //Graphicsオブジェクトのリソースを解放する
                g.Dispose();
                return canvas;
            }
        }

        public async Task<Image> GetZoomImageFromFile(List<string> FileList, string fileName, Size imgSize, bool zip)
        {
            Image img = null;
            if (zip)
            {
                int index = FileList.FindIndex(file => file == fileName);
                var ZipArc = ZipArcEntryList[index];
                using (var zipStream = ZipArc.Open())
                {
                    await Task.Run(() => img = GetZoomImageFromStream(zipStream, imgSize));
                }
            }
            else
            {
                using (FileStream fs = File.OpenRead(fileName))
                {
                    await Task.Run(() => img = GetZoomImageFromStream(fs, imgSize));
                }
            }
            return img;
        }
    }
}
