using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SpriteCanvas
{
    public class SpriteCanvas : Control
    {
        private int AnimationSteps = 1;

        protected override bool DoubleBuffered { get; set; } = true;

        /// <summary>
        /// Interval in miliseconds between each animation frame. How fast animation will go.
        /// </summary>
        public int SpriteSwitchInterval { get; set; } = 100;

        /// <summary>
        /// If set to true will resize input image to Control size
        /// </summary>
        public enum ResizeType
        {
            Fill,
            KeepWidth,
            KeepHeight,
            None
        }
        public ResizeType ResizeImage { get; set; } = ResizeType.None;

        public SpriteAnimation animation { get; private set; }

        private System.Windows.Forms.Timer _timer;
        private Bitmap _buffer;
        private int _animationStep = 0;

        public delegate void AnimationEnd();
        /// <summary>
        /// Called if animation.Loop == false
        /// </summary>
        public event AnimationEnd OnAnimationEnd;

        public SpriteCanvas()
        {
            _buffer = new Bitmap(2000, 2000, PixelFormat.Format32bppArgb);
        }

        public void AnimationStart(SpriteAnimation spriteAnimation)
        {
            if (_timer != null) _timer.Dispose();

            animation = spriteAnimation;

            AnimationSteps = animation.Length;

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = SpriteSwitchInterval;


            _timer.Tick += (s, e) =>
            {
                _animationStep += 1;

                

                if (_animationStep >= AnimationSteps)
                {
                    if (animation.Loop == true)
                    {
                        _animationStep = 0;
                    }
                    else
                    {
                        _animationStep = AnimationSteps - 1;
                        OnAnimationEnd?.Invoke();
                    }
                }

                Draw(animation[_animationStep]);

                this.Invalidate();
            };

            _timer.Start();
        }
        
        /// <summary>
        /// Draws single image instead of animation.
        /// </summary>
        /// 
        public void DrawImage(Image image)
        {
            if (_timer != null) _timer.Dispose();

            animation = new NullAnimation("null", null);

            Draw(image);
        }
        private void Draw(Image image)
        {
            if (image == null) return;


            float ratio = (float)image.Width / (float)image.Height;

            using (var g = Graphics.FromImage(_buffer))
            {
                g.Clear(Color.Transparent);
                // draw sprites based on current _animationStep value
                // g.DrawImage(...)
                
                if (ResizeImage == ResizeType.None)
                {
                    g.DrawImage(image, new PointF(0, 0));
                }
                if (ResizeImage == ResizeType.Fill)
                {
                    g.DrawImage(image, new Rectangle(0, 0, Width, Height));
                }
                if (ResizeImage == ResizeType.KeepWidth)
                {
                    g.DrawImage(image, new Rectangle(0, 0, Width, (int)(Width / ratio)));
                }
                if (ResizeImage == ResizeType.KeepHeight)
                {
                    g.DrawImage(image, new Rectangle(0, 0, (int)(Height / ratio), Height));
                }
            }
            this.Invalidate();

            //e.Graphics.DrawImage(_buffer, new Rectangle(0, 0, _buffer.Width, _buffer.Height), new Rectangle(0, 0, _buffer.Width, _buffer.Height), GraphicsUnit.Pixel);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_buffer, new Rectangle(0, 0, _buffer.Width, _buffer.Height), new Rectangle(0, 0, _buffer.Width, _buffer.Height), GraphicsUnit.Pixel);
        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_timer != null) _timer.Dispose();
            _buffer.Dispose();
        }
    }
}
