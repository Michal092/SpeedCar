using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Speed_Car
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ImageBrush playerSprite = new ImageBrush();
        ImageBrush backgroundSprite = new ImageBrush();
        ImageBrush cars1Sprite = new ImageBrush();
        ImageBrush cars2Sprite = new ImageBrush();
        ImageBrush cars3Sprite = new ImageBrush();
        ImageBrush sheldSprite = new ImageBrush();
        ImageBrush heartSprite = new ImageBrush();

        SoundPlayer gameSound = new SoundPlayer(@"gameSound.wav");

        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer sheldTimer = new DispatcherTimer();

        public bool right;
        public bool left;
        public bool turbo;
        public bool canColisions = true;

        public int distance;
        public int speed;
        public int boostTime;
        public int live = 3;

        public MainWindow()
        {
            InitializeComponent();
            timer.Tick += GameEngine;
            timer.Interval = TimeSpan.FromMilliseconds(5);
            sheldTimer.Tick += BoostEngine;
            sheldTimer.Interval = TimeSpan.FromMilliseconds(10);
            timer.Start();
            NewGame();
        }
        private void NewGame()
        {
            myCanvas.Focus();
            gameSound.PlayLooping();
            boost.Content = String.Empty;

            speed = 10;
            distance = 0;

            right = false;
            left = false;

            backgroundSprite.ImageSource = new BitmapImage(new Uri(@"road.png", UriKind.Relative));
            background1.Fill = backgroundSprite;
            background2.Fill = backgroundSprite;

            sheldSprite.ImageSource = new BitmapImage(new Uri(@"sheld.png", UriKind.Relative));
            sheld.Fill = sheldSprite;
            heartSprite.ImageSource = new BitmapImage(new Uri(@"hearts.png", UriKind.Relative));
            heart.Fill = heartSprite;

            cars1Sprite.ImageSource = new BitmapImage(new Uri(@"car1.png", UriKind.Relative));
            car1.Fill = cars1Sprite;
            cars2Sprite.ImageSource = new BitmapImage(new Uri(@"car2.png", UriKind.Relative));
            car2.Fill = cars2Sprite;
            car4.Fill = cars2Sprite;
            cars3Sprite.ImageSource = new BitmapImage(new Uri(@"car3.png", UriKind.Relative));
            car3.Fill = cars3Sprite;

            playerSprite.ImageSource = new BitmapImage(new Uri(@"cars.png", UriKind.Relative));
            player.Fill = playerSprite;

            Canvas.SetLeft(player, 120); Canvas.SetTop(player, 400);
            Canvas.SetLeft(car1, 137); Canvas.SetTop(car1, -300);
            Canvas.SetLeft(car2, 225); Canvas.SetTop(car2, 100);
            Canvas.SetLeft(car3, 320); Canvas.SetTop(car3, -900);
            Canvas.SetLeft(car4, 410); Canvas.SetTop(car4, 400);
        }

        private void GameEngine(object sender, EventArgs e)
        {
            distance += 10;
            Speed.Content = "Speed: " + speed;
            Distance.Content = "Distance: " + distance;
            Live.Content = "Lives: " + live;

            if (distance % 5000 == 0)
            {
                Canvas.SetLeft(sheld, 200);
                Canvas.SetTop(sheld, -200);
            }

            if (distance % 7000 == 0)
            {
                Canvas.SetLeft(heart, 200);
                Canvas.SetTop(heart, -200);
            }

            if (distance % 5000 == 0)
            {
                speed++;
            }

            Canvas.SetTop(background1, Canvas.GetTop(background1) + speed);
            Canvas.SetTop(background2, Canvas.GetTop(background2) + speed);

            Canvas.SetTop(car1, Canvas.GetTop(car1) + speed);
            Canvas.SetTop(car2, Canvas.GetTop(car2) + speed);
            Canvas.SetTop(car3, Canvas.GetTop(car3) + speed);
            Canvas.SetTop(car4, Canvas.GetTop(car4) + speed);
            Canvas.SetTop(sheld, Canvas.GetTop(sheld) + speed);
            Canvas.SetTop(heart, Canvas.GetTop(heart) + speed);

            if (Canvas.GetTop(car1) >= 700) { Canvas.SetTop(car1, Canvas.GetTop(car1) - 1000); }
            if (Canvas.GetTop(car2) >= 700) { Canvas.SetTop(car2, Canvas.GetTop(car1) - 600); }
            if (Canvas.GetTop(car3) >= 700) { Canvas.SetTop(car3, Canvas.GetTop(car1) - 1600); }
            if (Canvas.GetTop(car4) >= 700) { Canvas.SetTop(car4, Canvas.GetTop(car1) - 1300); }

            if (Canvas.GetTop(background1) > 650)
            {
                Canvas.SetTop(background1, Canvas.GetTop(background2) - background2.Height);
            }

            if (Canvas.GetTop(background2) > 650)
            {
                Canvas.SetTop(background2, Canvas.GetTop(background1) - background1.Height);
            }

            if (Canvas.GetLeft(player) <= 100)
            {
                Canvas.SetLeft(player, 100);
            }

            if (Canvas.GetLeft(player) >= 450)
            {
                Canvas.SetLeft(player, 450);
            }

            if (right)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 13);
            }

            if (left)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 13);
            }

            if (live <= 0)
            {
                MessageBoxResult message = MessageBox.Show("Game Over", "Crush", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (message == MessageBoxResult.Yes)
                {
                    live = 3;
                    NewGame();
                }
                if (message == MessageBoxResult.No)
                {
                    this.Close();
                }
            }

            CheckColisions();
        }


        private void CheckColisions()
        {
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "obstacle" && canColisions)
                {
                    Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect platformHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(platformHitBox))
                    {
                        live--;
                        MessageBoxResult message = MessageBox.Show("NewGame", "Crush", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (message == MessageBoxResult.Yes)
                        {
                            NewGame();
                        }
                        if (message == MessageBoxResult.No)
                        {
                            this.Close();
                        }
                    }
                }

                if ((string)x.Tag == "sheld")
                {
                    Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect platformHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (playerHitBox.IntersectsWith(platformHitBox))
                    {
                        Canvas.SetLeft(sheld, 900);
                        canColisions = false;
                        boostTime = 250;
                        sheldTimer.Start();
                    }
                }

                if ((string)x.Tag == "heart")
                {
                    Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect platformHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (playerHitBox.IntersectsWith(platformHitBox))
                    {
                        Canvas.SetLeft(heart, 900);
                        live++;
                    }
                }
            }
        }

        private void BoostEngine(object sender, EventArgs e)
        {
            boost.Content = "Boosts time:" + boostTime;
            boost.Background = Brushes.Beige;
            boostTime--;

            if (boostTime == 0)
            {
                canColisions = true;
                sheldTimer.Stop();
                boost.Background = Brushes.Transparent;
                boost.Content = String.Empty;
            }
        }

        private void myCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                right = true;
            }

            if (e.Key == Key.Left || e.Key == Key.A)
            {
                left = true;
            }
        }

        private void myCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                right = false;
            }

            if (e.Key == Key.Left || e.Key == Key.A)
            {
                left = false;
            }
        }
    }
}
