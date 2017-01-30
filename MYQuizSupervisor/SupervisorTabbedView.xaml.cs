using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace MYQuizSupervisor
{
    public partial class SupervisorTabbedView : TabbedPage
    {
        public SupervisorTabbedView()
        {
            InitializeComponent();

            var list = new ObservableCollection<ListItem>();

            list.Add(new ListItem { Title = "Item 1" });
            list.Add(new ListItem { Title = "Item 2" });

            repeater.ItemsSource = list;
            autoComplete.Suggestions = list;
        }
    }

    public class ListItem
    {
        public string Title { get; set; }
    }
}
