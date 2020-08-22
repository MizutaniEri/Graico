using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FindAssocExe;
using System.Drawing.Imaging;

namespace Graico
{
    public partial class Form1 : Form
    {
        private AsyncGraphicRead AsyncGraphReader = new AsyncGraphicRead();
        private List<string> FileList = null;
        private string nowFile;
        private bool zipFile;
        private Form2 ProgressForm = new Form2();
        private string ZipFileName;
        private List<PictureBox> PicBoxList = new List<PictureBox>();
        private List<Image> PictureList = new List<Image>();
        private PictureBox pictureBox = new PictureBox();
        private Stopwatch KeyTimeSW = new Stopwatch();
        private bool exec;
        private Point mouseDownLocation;
        private bool VolBtnNext;
        private string[] graphicFileExt = { ".jpg", ".jpe", ".jpeg", ".gif", ".bmp", ".png", ".tif", ".tiff" };


        public Form1()
        {
            InitializeComponent();

            MouseDown += picBox_MouseDown;
            MouseUp += picBox_MouseUp;
            PreviewKeyDown += Form1_PreviewKeyDown;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var cmdLine = System.Environment.GetCommandLineArgs();
            bool exeExec = false;
            cmdLine.ForEach(async cmd =>
            {
                int jumpNo = 0;
                string exeName = cmd.ToLower();
                if (exeName.EndsWith("graico.exe") ||
                    exeName.EndsWith("graico.vshost.exe"))  // VS Debug用
                {
                    exeExec = true;
                }
                else if (exeExec)
                {
                    string ext = Path.GetExtension(cmd).ToLower();
                    if ((ext == ".zip" || ext == ".cbz") ||
                        graphicFileExt.Contains(ext) &&
                        File.Exists(cmd))
                    {
                        await OpenGrapicFile(cmd);
                        if (FileList == null || FileList.Count <= 0)
                        {
                            // 既定の方法で開く
                            var proc = System.Diagnostics.Process.Start(cmd);
                            var procName = Path.GetFileName(Path.GetExtension(cmd).FindAssociatedExecutable()).ToLower();
                            var myProcName = Path.GetFileName(System.Environment.GetCommandLineArgs()[0]).ToLower();
                            if (procName == myProcName)
                            {
                                proc.Kill();
                            }
                            Close();
                            return;
                        }
                        else if (ext != ".zip" && ext != ".cbz")
                        {
                            jumpNo = FileList.FindIndex(fileName => fileName == cmd);
                        }
                    }
                    else if (cmd.IsNumeric())
                    {
                        string cmdNo = cmd;
                        jumpNo = Convert.ToInt32(cmdNo);
                    }
                    if (FileList != null && FileList.Count > 0)
                        await SetJumpImage(jumpNo);
                }
            });
        }

        public async Task OpenGrapicFile(string file)
        {
            if (ProgressForm.IsDisposed)
            {
                ProgressForm = new Form2();
            }
            ProgressForm.Show();

            // 指定されたファイルを元にZIPアーカイブ内／フォルダ内ファイル一覧を取得
            FileList = await AsyncGraphReader.GetFileListAsync(file);
            if (FileList != null && FileList.Count > 0)
            {
                ZipFileName = file;
                Debug.WriteLine("File List Count=" + FileList.Count);
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".zip" || ext == ".cbz")
                {
                    zipFile = true;
                    nowFile = FileList[0];
                }
                else
                {
                    zipFile = false;
                    nowFile = file;
                }
            }
            ProgressForm.Close();
        }

        /// <summary>
        /// 画面からPictureBoxを削除し、メモリも解放する
        /// </summary>
        /// <param name="picBoxList">PictureBoxリスト</param>
        private void PicBoxListClear(List<PictureBox> picBoxList)
        {
            if (picBoxList.Count > 0)
            {
                picBoxList.ForEach(pic =>
                {
                    pic.Image.Dispose();
                    pic.Image = null;
                    this.Controls.Remove(pic);
                });
                picBoxList.Clear();
            }
        }

        /// <summary>
        /// PictureBoxプロパティ設定
        /// </summary>
        /// <param name="picBox">設定するPictureBox</param>
        /// <param name="img">PictureBoxに設定するイメージ</param>
        /// <param name="yPos">PictureBox表示開始縦位置(横位置は0固定)</param>
        private void SetPictureBoxProperty(PictureBox picBox, Image img, int yPos)
        {
            picBox.Location = new Point(0, yPos);
            picBox.Image = img;
            picBox.Size = new Size(img.Width, img.Height);
            picBox.SizeMode = PictureBoxSizeMode.CenterImage;
            picBox.Enabled = true;
            picBox.Visible = true;
            picBox.ContextMenuStrip = contextMenuStrip1;
            int index = FileList.FindIndex(file => file == nowFile) + 1;
            picBox.DoubleClick += async (sender, e) => await GetNextImage(index);
            picBox.MouseDown += picBox_MouseDown;
            picBox.MouseUp += picBox_MouseUp;
        }

        private void picBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //this.OnMouseUp(e);
                return;
            }
            mouseDownLocation = e.Location;
            Debug.WriteLine("Mouse Down Event. Location=" + mouseDownLocation.ToString());
        }

        private async void picBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //this.OnMouseUp(e);
                return;
            }
            Debug.WriteLine("Mouse Up Event. Location=" + e.Location.ToString());
            int mouseMoveX = e.X - mouseDownLocation.X;
            //int mouseMoveY = e.Y - mouseDownLocation.Y;
            //int maxVertScroll = VerticalScroll.Maximum - VerticalScroll.LargeChange;
            //int maxHoriScroll = HorizontalScroll.Maximum - HorizontalScroll.LargeChange;
            //bool scroll = false;
            //try
            //{
            //    if (VerticalScroll.Value < maxVertScroll && mouseMoveY != 0)
            //    {
            //        VerticalScroll.Value += mouseMoveY;
            //        scroll = true;
            //    }
            //    if (HorizontalScroll.Value < maxHoriScroll && mouseMoveX != 0)
            //    {
            //        HorizontalScroll.Value += mouseMoveX;
            //        scroll = true;
            //    }
            //    if (scroll)
            //    {
            //        return;
            //    }
            //}
            //catch {}
            // 横方向に移動なし、または、移動距離が短い
            if (mouseMoveX == 0 || Math.Abs(mouseMoveX) < 100)
            {
                return;
            }
            if (mouseMoveX > 0)
            {
                await mouseSwipe(1);
            }
            else
            {
                await mouseSwipe(-1);
            }
        }

        /// <summary>
        /// イメージリスト画像フォーム貼り付け
        /// </summary>
        /// <param name="imgList">フォーム貼り付けるイメージリスト</param>
        private void PicBoxSetImage(List<Image> imgList)
        {
            int yPos = 0;
            try
            {
                if (imgList.Count <= 0) return;
                Size imgSize = new Size(imgList[0].Size.Width, 0);
                imgList.ForEach(img =>
                {
                    var picBox = new PictureBox();
                    SetPictureBoxProperty(picBox, img, yPos);
                    imgSize.Height += img.Height;
                    this.Controls.Add(picBox);
                    PicBoxList.Add(picBox);
                    yPos += img.Height;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 次の画像読み込みと関連する処理の実施
        /// </summary>
        /// <param name="index">表示インデックス</param>
        /// <returns>戻り値なし(待機可能にするための指定->voidにすると待機なしになる)</returns>
        private async Task GetNextImage(int index)
        {
            try
            {
                if (ProgressForm.IsDisposed)
                {
                    ProgressForm = new Form2();
                }
                ProgressForm.Show();
                PicBoxListClear(PicBoxList);
                ImageListClear(PictureList);
                nowFile = await AsyncGraphReader.GetIndexImage(PictureList, FileList, index, zipFile);
                if (PictureList.Count > 0)
                {
                    PicBoxSetImage(PictureList);
                    int newIndex = FileList.FindIndex(file => file == nowFile) + 1;
                    SetMainTitle(newIndex);
                }
                ProgressForm.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var openFile = new OpenFileDialog();
            openFile.Filter = "Graphic Files|*.jpg;*.jpeg;*.bmp;*.gif;*.png;*.zip;*.cbz";
            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string file = openFile.FileName;
                await OpenGrapicFile(file);
                if (FileList.Count <= 0)
                {
                    MessageBox.Show("画像ファイルが存在しません! File:" + file);
                }
                if (FileList != null && FileList.Count > 0)
                    await SetJumpImage(0);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            bool viewOk = false;
            if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.Enter) ||
                (e.KeyCode == Keys.Left || e.KeyCode == Keys.Back))
            {
                if (KeyTimeSW.IsRunning)
                {
                    Debug.WriteLine("Time = " + KeyTimeSW.ElapsedMilliseconds + "ms");
                    if (KeyTimeSW.ElapsedMilliseconds > 200)
                    {
                        viewOk = true;
                        KeyTimeSW.Restart();
                    }
                    else
                    {
                        viewOk = false;
                    }

                }
                else
                {
                    KeyTimeSW.Start();
                    viewOk = true;
                }
                //Debug.WriteLine("View=" + (viewOk ? "OK" : "NG"));
                if (!viewOk)
                {
                    return;
                }
            }
            int addIndex = 0;
            if (VolBtnNext && e.KeyCode == Keys.VolumeDown)
            {
                addIndex = 1;
                await SetPicBoxSizeMode(addIndex);
                e.IsInputKey = false;
            }
            else if (VolBtnNext && e.KeyCode == Keys.VolumeUp)
            {
                addIndex = -1;
                await SetPicBoxSizeMode(addIndex);
                e.IsInputKey = false;
            }
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.Enter)
            {
                Debug.WriteLine("============= Rigth or Enter Input! =============");
                addIndex = 1;
                //await SetPicBoxSizeMode(addIndex);
            }
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Back)
            {
                Debug.WriteLine("============= Left or Back Input! =============");
                addIndex = -1;
            }
            if ((e.KeyCode == Keys.Right || e.KeyCode == Keys.Enter) ||
                (e.KeyCode == Keys.Left || e.KeyCode == Keys.Back))
            {
                //var findIndex = FileList.FindIndex(file => file == nowFile) + addIndex + 1;
                //if (findIndex <= 0)
                //{
                //    findIndex = FileList.Count;
                //}
                //else if (findIndex > FileList.Count)
                //{
                //    findIndex = 1;
                //}
                //await GetNextImage(-1);
                //await SetJump(-1);
                //Debug.WriteLine("KeyPreview index=" + findIndex);
                //await SetPicBoxSizeMode(findIndex);
                await SetPicBoxSizeMode(addIndex);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
            else if (e.KeyCode == Keys.Space)
            {
                int newX = (ClientSize.Width / 2) - (contextMenuStrip1.Width / 2);
                int newY = (ClientSize.Height / 2) - (contextMenuStrip1.Height / 2);
                contextMenuStrip1.Show(this, new Point(newX, newY));
            }
        }

        private void SetMainTitle(int index)
        {
            Text = Path.GetFileName(ZipFileName) + " - " + nowFile + " (" + (index + 1) + "/" + FileList.Count + ")";
        }

        private void ImageListClear(List<Image> ImageList)
        {
            ImageList.ForEach(img => img.Dispose());
            ImageList.Clear();
        }

        public async Task SetJumpImage(int index)
        {
            if (ProgressForm.IsDisposed)
            {
                ProgressForm = new Form2();
            }
            ProgressForm.Show();
            ImageListClear(PictureList);
            nowFile = await AsyncGraphReader.GetIndexImage(PictureList, FileList, index, zipFile);
            if (nowFile == null)
            {
                return;
            }
            if (PictureList.Count > 0)
            {
                PicBoxListClear(PicBoxList);
            }
            PicBoxSetImage(PictureList);
            Debug.WriteLine("SetJumpImage indx = " + index + " Now file = " + nowFile);
            SetMainTitle(index);
            ProgressForm.Close();
        }

        private async void jumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int index = FileList.FindIndex(file => file == nowFile);
            var jumpInputForm = new Form3();
            jumpInputForm.setNumericUpDownMaxValue(FileList.Count);
            jumpInputForm.setNumericUpDownValue(index + 1);
            if (jumpInputForm.ShowDialog() == DialogResult.OK)
            {
                index = jumpInputForm.getNumericUpDownValue();
                //await SetJumpImage(index-1);
                await SetPicBoxSizeMode(index - 1, false);
            }
        }

        private async void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int index = FileList.FindIndex(file => file == nowFile) + 1;
            //await GetNextImage(index);
            await SetPicBoxSizeMode(1);
        }

        private async void beforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //int index = FileList.FindIndex(file => file == nowFile) - 1;
            //await GetNextImage(index);
            await SetPicBoxSizeMode(-1);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nowFile))
            {
                return;
            }
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Graphic Files|*.jpg;*.jpeg;*.bmp;*.gif;*.png|JPEG|*.jpg|PNG|*.png|Bitmap|*.bmp";
            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                ImageFormat imgform = null;
                string file = saveDialog.FileName;
                string ext = Path.GetExtension(file).ToLower();
                if (ext == ".jpg" || ext == "*.jpe" || ext == ".jpeg")
                {
                    imgform = ImageFormat.Jpeg;
                }
                else if (ext == ".bmp")
                {
                    imgform = ImageFormat.Bmp;
                }
                else if (ext == ".png")
                {
                    imgform = ImageFormat.Png;
                }
                Image.FromFile(nowFile).Save(file, imgform);
            }
        }

        private async Task SetJump(int indexAdd = 0)
        {
            await SetJumpImage(nextFileIndex(indexAdd));
        }

        private int nextFileIndex(int index)
        {
            var findIndex = FileList.FindIndex(file => file == nowFile) + index;
            if (findIndex < 0)
            {
                findIndex = FileList.Count - 1;
            }
            else if (findIndex >= FileList.Count)
            {
                findIndex = 0;
            }
            return findIndex;
        }

        private async Task SetPicBoxSizeMode(int index, bool next = true)
        {
            if (ProgressForm.IsDisposed)
            {
                ProgressForm = new Form2();
            }
            ProgressForm.Show();
            // ズームモード
            // サイズが小さくなるので、分割なしにする
            var phMen = new Microsoft.VisualBasic.Devices.ComputerInfo().AvailablePhysicalMemory;
            if ((System.Environment.Is64BitOperatingSystem &&
                phMen >= (4L * 1024L * 1024L * 1024L)) ||
                (zoomToolStripMenuItem.Checked || wideFitZoomToolStripMenuItem.Checked))
            {
                var findIndex = FileList.FindIndex(file => file == nowFile);
                if (next)
                {
                    findIndex = nextFileIndex(index);
                }
                // PictureBoxリストクリア
                // まずはコントロールコレクションから
                ControlsPicBoxClear(PicBoxList);
                // リスト自体の削除
                PicBoxListClear(PicBoxList);
                // イメージリストも削除
                ImageListClear(PictureList);
                // ここから、新たに読み込みと追加
                //nowFile = await AsyncGraphReader.GetIndexImage(PictureList, FileList, findIndex, zipFile, 0);
                //SetMainTitle(findIndex);
                //PicBoxSetImage(PictureList);
                //nowFile = FileList[index - 1];
                //SetPictureBoxProperty(pictureBox, PictureList[0], 0);
                //pictureBox = PicBoxList[0];
                Size newSize;
                AsyncGraphReader.GetPictureSize(FileList, FileList[findIndex], out newSize, zipFile);
                if (zoomToolStripMenuItem.Checked)
                {
                    GetScreenFitSize(ref newSize);

                    var newImage = await AsyncGraphReader.GetZoomImageFromFile(FileList, FileList[findIndex], newSize, zipFile);
                    nowFile = FileList[findIndex];
                    SetMainTitle(findIndex);
                    pictureBox.Image = newImage;
                    if (zoomToolStripMenuItem.Checked)
                    {
                        pictureBox.Size = ClientSize;
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        pictureBox.Size = newImage.Size;
                    }
                    pictureBox.Location = new Point(0, 0);
                    pictureBox.MouseDown += picBox_MouseDown;
                    pictureBox.MouseUp += picBox_MouseUp;
                    Controls.Add(pictureBox);
                    pictureBox.Refresh();
                }
                else if (wideFitZoomToolStripMenuItem.Checked)
                {
                    GetScreenWideFitSize(ref newSize);

                    var newImage = await AsyncGraphReader.GetZoomImageFromFile(FileList, FileList[findIndex], newSize, zipFile);
                    nowFile = FileList[findIndex];
                    SetMainTitle(findIndex);
                    AutoScrollPosition = new Point(0, 0);
                    pictureBox.Image = newImage;
                    pictureBox.Size = newImage.Size;
                    pictureBox.Location = new Point(0, 0);
                    pictureBox.MouseDown += picBox_MouseDown;
                    pictureBox.MouseUp += picBox_MouseUp;
                    Controls.Add(pictureBox);
                    pictureBox.Refresh();
                }
                else
                {
                    if (pictureBox.Image != null)
                    {
                        pictureBox.Image.Dispose();
                        pictureBox.Image = null;
                    }
                    if (Controls.Contains(pictureBox))
                        Controls.Remove(pictureBox);
                    if (next)
                    {
                        await SetJump(index);
                    }
                    else
                    {
                        await SetJumpImage(index);
                    }
                }
                ProgressForm.Close();
            }
        }

        /// <summary>
        /// 画面のコントロールコレクションからPicBoxListのアイテムを削除する
        /// </summary>
        /// <param name="PicBoxList">削除対象のリスト</param>
        private void ControlsPicBoxClear(List<PictureBox> PicBoxList)
        {
            PicBoxList.ForEach(picBox =>
            {
                if (Controls.Contains(picBox))
                {
                    Debug.WriteLine("Remove PictureBox=" + picBox.Size.ToString());
                    Controls.Remove(picBox);
                }
            });
        }

        private async void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (zoomToolStripMenuItem.Checked && wideFitZoomToolStripMenuItem.Checked)
            {
                wideFitZoomToolStripMenuItem.Checked = false;
            }
            var findIndex = FileList.FindIndex(file => file == nowFile);
            if (findIndex < 0)
            {
                findIndex = FileList.Count - 1;
            }
            else if (findIndex > FileList.Count)
            {
                findIndex = 1;
            }
            await SetPicBoxSizeMode(findIndex, false);
        }

        private static void GetScreenFitSize(ref Size imageSize)
        {
            // スクリーンサイズの取得
            Rectangle Rect = Screen.GetWorkingArea(new Point(0, 0));
            // スクリーンサイズの幅と高さ
            int screenX = Rect.Size.Width;
            int screenY = Rect.Size.Height;
            // 画像の幅と高さ
            int imageX = imageSize.Width;
            int imageY = imageSize.Height;
            int newX = 0;
            int newY = 0;
            int RX = 0;
            int RY = 0;

            // 画像の比率に沿った幅と高さ計算
            RX = imageX * screenY / imageY;
            RY = imageY * screenX / imageX;

            if ((RX < screenX) && (RY > screenY))
            {
                newX = RX;
                newY = screenY;
            }
            else
            {
                newX = screenX;
                newY = RY;
            }
            imageSize.Width = newX;
            imageSize.Height = newY;
        }
 
        private static void GetScreenWideFitSize(ref Size imageSize)
        {
            // スクリーンサイズの取得
            Rectangle Rect = Screen.GetWorkingArea(new Point(0, 0));
            // スクリーンサイズの幅と高さ
            int screenX = Rect.Size.Width;
            int screenY = Rect.Size.Height;
            // 画像の幅と高さ
            int imageX = imageSize.Width;
            int imageY = imageSize.Height;
            int newX = 0;
            int newY = 0;
            int RX = 0;
            int RY = 0;

            // 画像の比率に沿った幅と高さ計算
            RX = imageX * screenY / imageY;
            RY = imageY * screenX / imageX;
            // 横幅はスクリーンサイズ固定
            newY = RY;
            newX = screenX;
            imageSize.Width = newX;
            imageSize.Height = newY;
        }

        private async Task mouseSwipe(int indexAdd)
        {
            // 2重起動防止(1回の処理中に同イベントが発生しても何もしない)
            if (exec)
            {
                return;
            }
            exec = true;
            await SetPicBoxSizeMode(indexAdd);
            exec = false;
        }

        private void Form1_Scroll(object sender, ScrollEventArgs e)
        {
            Debug.WriteLine("" + e.ScrollOrientation);
            Debug.WriteLine("Scroll=" + e.NewValue);
            Debug.WriteLine("VScroll=" + VerticalScroll.Value);
            Debug.WriteLine("HScroll=" + HorizontalScroll.Value);
            Debug.WriteLine("VLarge Scroll=" + VerticalScroll.LargeChange);
            Debug.WriteLine("HLarge Scroll=" + HorizontalScroll.LargeChange);
            Debug.WriteLine("VSmall Scroll=" + VerticalScroll.SmallChange);
            Debug.WriteLine("HSmall Scroll=" + HorizontalScroll.SmallChange);
            Debug.WriteLine("VerticalScroll.Maximum=" + VerticalScroll.Maximum);
            Debug.WriteLine("HorizontalScroll.Maximum=" + HorizontalScroll.Maximum);
        }

        private void fullscreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fullscreenToolStripMenuItem.Checked)
            {
                WindowState = FormWindowState.Normal;
                FormBorderStyle = FormBorderStyle.None;
                TopMost = true;
                WindowState = FormWindowState.Maximized;
            }
            else
            {
                WindowState = FormWindowState.Maximized;
                FormBorderStyle = FormBorderStyle.Sizable;
                TopMost = false;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(pictureBox.Image);
        }

        private void volButtonEnableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (volButtonEnableToolStripMenuItem.Checked)
            {
                VolBtnNext = true;
            }
            else
            {
                VolBtnNext = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up)
            {
                this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, -this.AutoScrollPosition.Y - 10);
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, -this.AutoScrollPosition.Y + 10);
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, -this.AutoScrollPosition.Y - 100);
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, -this.AutoScrollPosition.Y + 100);
            }
            else if (e.KeyCode == Keys.Home)
            {
                this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, 0);
            }
            else if (e.KeyCode == Keys.End)
            {
                this.AutoScrollPosition = new Point(this.AutoScrollPosition.X, 30000);
            }
        }

        private async void wideFitZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (zoomToolStripMenuItem.Checked && wideFitZoomToolStripMenuItem.Checked)
            {
                zoomToolStripMenuItem.Checked = false;
            }
            var findIndex = FileList.FindIndex(file => file == nowFile);
            if (findIndex < 0)
            {
                findIndex = FileList.Count - 1;
            }
            else if (findIndex > FileList.Count)
            {
                findIndex = 1;
            }
            await SetPicBoxSizeMode(findIndex, false);
        }
    }
}
