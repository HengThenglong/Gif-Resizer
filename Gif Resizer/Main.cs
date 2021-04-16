using ImageMagick;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gif_Resizer
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        FileInfo File;
        private void BtnOpenFile_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Pick an Animated Gif",
                Filter = "Gif Image file (*.gif)|*.gif",
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                PicBox.Image = new Bitmap(openFileDialog.FileName);
                File = new FileInfo(openFileDialog.FileName);
                EnableConvertBtn();
                BtnOpenFile.Hide();


                using (var collection = new MagickImageCollection(File))
                {
                    var firstFrame = collection[0];
                    var width = firstFrame.Width;
                    var height = firstFrame.Height;

                    NumericUpDownWidth.Value = width;
                    NumericUpDownHeight.Value = height;
                }
            }
        }

        private void BtnConvert_Click(object sender, EventArgs e)
        {



            // Read from file
            using (var collection = new MagickImageCollection(File))
            {
                // This will remove the optimization and change the image to how it looks at that point
                // during the animation. More info here: http://www.imagemagick.org/Usage/anim_basics/#coalesce
                collection.Coalesce();

                // Resize each image in the collection to a width of 200. When zero is specified for the height
                // the height will be calculated with the aspect ratio.
                foreach (var image in collection)
                {
                    //image.Resize(200, 200);
                    image.Resize((int)NumericUpDownWidth.Value, (int)NumericUpDownHeight.Value);
                }

                Directory.CreateDirectory(File.Directory + "\\Converted");
                File = new FileInfo($"{File.Directory}\\Converted\\{File.Name}");
                // Save the result
                //collection.Write($"{File}_Convert.gif");
                collection.Write(File);

            }
        }

        Boolean CanConvert()
        {
            if (File != null && NumericUpDownWidth.Value > 0 && NumericUpDownHeight.Value > 0)
                return true;


            return false;
        }

        void EnableConvertBtn()
        {
            var canConvert = CanConvert();

            if (canConvert) BtnConvert.Enabled = true;
            else BtnConvert.Enabled = false;
        }

        private void NumericUpDownWidth_ValueChanged(object sender, EventArgs e)
        {
            EnableConvertBtn();

            if (File != null)
            {
                using (var collection = new MagickImageCollection(File))
                {


                    var firstFrame = collection[0];
                    var width = firstFrame.Width;
                    var height = firstFrame.Height;

                    if (NumericUpDownWidth.Value == width) return;

                    float aspectRatio = (float)height / width;
                    float newHeight = (float)aspectRatio * width;

                    NumericUpDownHeight.Value = (decimal)newHeight;
                }
            }
        }

        private void NumericUpDownHeight_ValueChanged(object sender, EventArgs e)
        {
            EnableConvertBtn();

            if (File != null)
            {
                using (var collection = new MagickImageCollection(File))
                {
                    var firstFrame = collection[0];
                    var width = firstFrame.Width;
                    var height = firstFrame.Height;

                    if (NumericUpDownHeight.Value == height) return;


                    float aspectRatio = (float)height / width;
                    float newWidth = (float)NumericUpDownHeight.Value / aspectRatio;

                    NumericUpDownHeight.Value = (decimal)newWidth;
                }
            }
        }


    }
}
