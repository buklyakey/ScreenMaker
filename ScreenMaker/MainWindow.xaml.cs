using Microsoft.Win32;
using System;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScreenMaker
{
    public partial class MainWindow : Window
    {

        private Point beginCapture;
        private Point endCapture;
        private bool drawing = false;
        private Rectangle r;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Нажатие правой кнопкой - выход
            if (e.ChangedButton == MouseButton.Right)
            {
                Close();
                return;
            }


            // Начинаем выделять область скриншота
            if (!drawing)
            {
                beginCapture = PointToScreen(Mouse.GetPosition(this));

                drawing = true;
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            // Выделение
            if (e.LeftButton == MouseButtonState.Pressed && drawing)
            {
                endCapture = PointToScreen(e.GetPosition(this));

                r = new Rectangle();
                Point bp = PointFromScreen(new Point(beginCapture.X, beginCapture.Y));
                Point ep = PointFromScreen(new Point(endCapture.X, endCapture.Y));

                r.SetValue(Canvas.LeftProperty, Math.Min(bp.X, ep.X));
                r.SetValue(Canvas.TopProperty, Math.Min(bp.Y, ep.Y));
                r.Height = Math.Abs(bp.Y - ep.Y) + 1;
                r.Width = Math.Abs(bp.X - ep.X) + 1;

                r.Fill = Brushes.Red;
                r.Opacity = 0.5;

                grid.Children.Clear();
                grid.Children.Add(r);
            }
            // Если левая кнопка отпущена, значит выделение завершено
            // делаем скриншот и предлагаем сохранить
            else if (drawing && e.LeftButton == MouseButtonState.Released)
            {
                drawing = false; // выделение завершено

                Hide();          // Скрыть форму скриншотера

                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap((int)r.Width, (int)r.Height))
                {
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(new System.Drawing.Point((int)beginCapture.X, (int)beginCapture.Y), System.Drawing.Point.Empty, new System.Drawing.Size((int)r.Width, (int)r.Height));
                    }

                    SaveFileDialog sfd = new SaveFileDialog
                    {
                        Filter = "PNG-Image (.png)|*.png",
                        FileName = "Screen"
                    };

                    // Если выбран путь, сохраняем и закрываем программу
                    // Иначе продолжается работа программы
                    if (sfd.ShowDialog() ?? false)
                    {
                        bitmap.Save(sfd.FileName, ImageFormat.Png);
                        Close();
                    }
                    else
                    {
                        Show();
                    }
                }
            }
        }
    }
}
