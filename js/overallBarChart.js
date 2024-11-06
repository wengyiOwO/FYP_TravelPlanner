document.addEventListener("DOMContentLoaded", function () {
    var ctx = document.getElementById('myBarChart').getContext('2d');
    var myBarChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ["Pulau Langkawi", "Desaru Resort", "Sunway Lagoon", "Legoland", "Pulau Redang", "Escape Park", "A'Famosa", "KLCC"],
            datasets: [{
                label: 'Number of Visits',
                data: [100, 90, 85, 75, 70, 60, 50, 45], 
                backgroundColor: 'rgba(78, 115, 223, 0.5)',
                borderColor: 'rgba(78, 115, 223, 1)',
                borderWidth: 1
            }]
        },
        options: {
            scales: {
                x: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Popular Destinations'
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Number of Visits'
                    }
                }
            },
            plugins: {
                legend: {
                    display: false
                }
            }
        }
    });
});
