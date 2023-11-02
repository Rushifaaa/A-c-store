var mode;

$(function () {
    // richtiges Objekt initialiseren
    if (mode != null)
        window[mode].init();
});

var ChartView = {
    mode: null, // muss in View gesetzt werden zuvor
    chartRawData: null,
    config: null,

    init: function () {
        ChartView.mode = chartMode;
        // Chart initialisieren
        ChartView.initChart()
    },
    initChart: function () {
        // Daten durch API lesen
        ChartView.getData();
    },
    getData: function () {
        // Ajax Call zum holen der Daten
        $.ajax({
            type: "GET",
            url: '/Chart/GetProductChartData?mode=' + ChartView.mode,
            success: function (data) {
                // Wenne Erfolg --> Daten merken
                ChartView.chartRawData = data;
                // Daten ins Chart-Format bringen
                ChartView.createChartData();
                // Chart zeigen
                ChartView.show();
            },
            error: function () {
                // Wenn Error --> Toast mit Fehler
                Layout.showToast(true, "Fehler beim Erstellen der Statistik!");
            }
        });
    },
    createChartData: function () {
        // lables & Data erstellen
        let lables = [];
        let data = [];
        ChartView.chartRawData.forEach(function (element) {
            lables.push(element.Name);
            data.push(element.Sales);
        });

        // generelle Daten erstellen
        let dataGenerel = {
            labels: lables,
            datasets: [{
                backgroundColor: ["blue", "red", "green", "yellow", "orange"], // Farben setzen
                data: data
            }],
        }

        // stepSize berechnen
        let biggestNumber = 0;
        ChartView.chartRawData.forEach(function (element) {
            if (biggestNumber < element.Sales)
                biggestNumber = element.Sales;
        });
        let stepSize = 0;
        if (biggestNumber <= 15)
            stepSize = 1;
        else if (biggestNumber <= 50)
            stepSize = 5;
        else if (biggestNumber <= 150)
            stepSize = 10;
        else if (biggestNumber <= 500)
            stepSize = 50;
        else if (biggestNumber <= 2000)
            stepSize = 100;
        else if (biggestNumber <= 10000)
            stepSize = 500;
        else if (biggestNumber <= 20000)
            stepSize = 1000;
        else if (biggestNumber <= 50000)
            stepSize = 5000;
        else if (biggestNumber <= 100000)
            stepSize = 10000;
        else if (biggestNumber <= 1000000)
            stepSize = 100000;
        else if (biggestNumber > 1000000)
            stepSize = 1000000;

        ChartView.config = {
            type: 'bar',
            data: dataGenerel,
            options: {
                legend: { display: false },
                title: {
                    display: true,
                    text: "Die am " + (ChartView.mode == 0 ? "meist" : "wenigsten") + " verkauften Spiele"
                },
                scales: {
                    yAxes: [{
                        ticks: {
                            beginAtZero: true,
                            stepSize: stepSize
                        }
                    }]
                }
            }
        };
    },
    show: function () {
        new Chart(
            "chart",
            ChartView.config
        );
    }
}
