using System.Collections;
using System.Drawing;

namespace SpriteCanvas
{
    public class SpriteAnimation : IEnumerable
    {
        public readonly string Name;
        private Image[] _images;

        public bool Loop = true;

        public int Length { get
            {
                return _images.Length;
            } }

        public SpriteAnimation(string name, Image[] images)
        {
            _images = images;
            Name = name;
        }
        public IEnumerator GetEnumerator()
        {
            return _images.GetEnumerator();
        }

        public Image this[int index]
        {
            get => _images?[index];
            set => _images[index] = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is SpriteAnimation sp)
            {
                return sp.Name.Equals(Name);
            }

            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class NullAnimation : SpriteAnimation
    {
        public readonly string Name = "null";

        public NullAnimation(string name, Image[] images): base(name, images)
        {

        }
        
    }
}
