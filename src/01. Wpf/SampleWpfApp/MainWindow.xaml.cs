using Diginsight.Diagnostics;
using Diginsight.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SampleWpfApp;

/// <summary>Interaction logic for MainWindow.xaml</summary>
public partial class MainWindow : Window
{
    static Type T = typeof(MainWindow);
    private readonly IServiceProvider serviceProvider;
    private ILogger<MainWindow> logger;

    static MainWindow()
    {
        var logger = App.ObservabilityManager.LoggerFactory.CreateLogger<MainWindow>();
        using var activity = Observability.ActivitySource.StartMethodActivity(logger);

        var host = App.Host;
    }
    public MainWindow()
    {
        InitializeComponent();
    }
    public MainWindow(
                         IServiceProvider serviceProvider,
                         ILogger<MainWindow> logger,
                         IHttpClientFactory httpClientFactory,
                         HttpClient httpClient,
                         IHttpContextAccessor httpContextAccessor)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { logger });


        InitializeComponent();
    }
    private void MainWindow_Initialized(object sender, EventArgs e)
    {
        using var activity = Observability.ActivitySource.StartMethodActivity(logger, new { sender, e });


    }
}