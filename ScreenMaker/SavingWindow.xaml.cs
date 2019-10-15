using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ScreenMaker
{
    /// <summary>
    /// Логика взаимодействия для SavingWindow.xaml
    /// </summary>
    public partial class SavingWindow : Window
    {
        private readonly Bitmap _img;
        private readonly BitmapImage _convertedImg;


        /// <summary>
        /// Конструктор для окна отображения и сохранения изображения
        /// </summary>
        /// <param name="image">Изображение (скриншот)</param>
        public SavingWindow(Bitmap image)
        {
            InitializeComponent();

            _img = image;
            _convertedImg = BitmapToImageSource(image);

            // Отображение изображения
            imgScreen.Source = _convertedImg;

            imgScreen.Height = image.Height;
            imgScreen.Width = image.Width;
        }

        // Сохранение изображения в файл
        private void SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                FileName = "Screen",
                Filter = "PNG-Image (.png)|*.png|JPEG-Image (.jpeg)|*.jpeg|BMP-Image (.bmp)|*.bmp"
            };

            if (sfd.ShowDialog() ?? false)
            {
                ImageFormat selectedFormat;

                switch (sfd.FilterIndex)
                {
                    case 0: selectedFormat = ImageFormat.Png; break;
                    case 1: selectedFormat = ImageFormat.Jpeg; break;
                    case 2: selectedFormat = ImageFormat.Bmp; break;
                    default: selectedFormat = ImageFormat.Png; break;
                }

                _img.Save(sfd.FileName, selectedFormat);
            }
        }


        // Конвертация изображения для отображения в WPF-Image
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        // Копировать изображение в буфер обмена
        private void SaveToClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage(_convertedImg);
            MessageBox.Show("Скопировано!", "OK", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Закрыть форму и сделать новый скриншот
        private void CreateNewImage_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}