using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Pogoda2;


public partial class MainWindow : Window
{
    private readonly HttpClient httpClient = new HttpClient();


    public MainWindow()
    {
        InitializeComponent();
    }


    private void getCords (object sender, MouseButtonEventArgs e)
    {
        // Disables the default mouse double-click action.
        e.Handled = true;

        //Get the mouse click coordinates
        var mousePosition = e.GetPosition(Map);

        //Convert the mouse coordinates to a locatoin on the map
        Location pinLocation = Map.ViewportPointToLocation(mousePosition);

        // The pushpin to add to the map.
        Pushpin pin = new Pushpin();
        pin.Location = pinLocation;
        LoadWeatherData(pinLocation.Longitude, pinLocation.Latitude);
    }

    private async void LoadWeatherData(double lon, double lat)
    {
        try
        {
            var response = await httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?lon={lon}&lat={lat}&units=metric&appid=f695b9814498876ee0220627a2fa06b7");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var weatherData = JObject.Parse(content);
                UpdateUI(weatherData);
            }
            else
            {
                MessageBox.Show("Nie udało sie pobrac danych pogodowych");

            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    private void UpdateUI(JObject weatherData)
    {
        var weatherDescription = weatherData["weather"][0]["description"].ToString();
        var temperature = Convert.ToDouble(weatherData["main"]["temp"]);
        var feelsLike = Convert.ToDouble(weatherData["main"]["feels"]);
        var humidity = Convert.ToInt32(weatherData["main"]["humidity"]);
        var clouds = Convert.ToInt32(weatherData["clouds"]["all"]);
        var rain = Convert.ToInt32(weatherData["main"]["rain"]);

        lblWeather.Text = $"Aktualna pogoda: {weatherDescription}";
        lblTemperature.Text = $"Temperatura: {temperature}°C";
        lblFeelsLike.Text = $"Odczuwalna temperatura: {feelsLike}°C";
        lblRain.Text = $"Wilgotność: {humidity}%";
        lblClouds.Text = $"Nasłonecznienie: {clouds}%";
        lblRain.Text = $"Deszcz: {rain}%";
    }
}