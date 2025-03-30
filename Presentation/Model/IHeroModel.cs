using System.ComponentModel;

namespace Presentation.Model
{
    internal interface IHeroModel : INotifyPropertyChanged
    {
        int HeroId { get; set; }
        string Name { get; set; }
        int Gold { get; set; }
    }
}
