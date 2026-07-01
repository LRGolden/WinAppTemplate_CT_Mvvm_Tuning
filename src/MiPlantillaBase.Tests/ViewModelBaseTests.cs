using MiPlantillaBase.Core;
using Xunit;

namespace MiPlantillaBase.Tests;

public sealed class ViewModelBaseTests
{
    private sealed class TestViewModel : ViewModelBase
    {
        public void NotifyProperty() => OnPropertyChanged(nameof(NotifyProperty));
    }

    [Fact]
    public void ViewModelBase_InheritsObservableObject_NotifiesPropertyChange()
    {
        var vm = new TestViewModel();
        var notified = false;

        vm.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(TestViewModel.NotifyProperty))
                notified = true;
        };

        vm.NotifyProperty();

        Assert.True(notified);
    }
}
