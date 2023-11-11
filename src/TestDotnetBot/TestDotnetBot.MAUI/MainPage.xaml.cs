using TestDotnetBot;

namespace TestDotnetBot.MAUI
{
    public partial class MainPage : ContentPage
    {
        private readonly MyApplication evergineApplication;
        private int count = 0;

        public MainPage()
        {
            InitializeComponent();
            this.evergineApplication = new MyApplication();
            this.evergineView.Application = this.evergineApplication;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}