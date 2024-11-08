document.addEventListener("DOMContentLoaded", function () {

    var ctx = document.getElementById("myPieChart").getContext('2d');


    var myPieChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ["1 star", "2 stars", "3 stars", "4 stars", "5 stars"],
            datasets: [{
                data: ratingData,
                backgroundColor: ['#e74a3b', '#f6c23e', '#36b9cc', '#1cc88a', '#4e73df'],
                hoverBackgroundColor: ['#be2617', '#dda20a', '#2c9faf', '#17a673', '#2e59d9'],
                hoverBorderColor: "rgba(234, 236, 244, 1)",
            }],
        },
        options: {
            maintainAspectRatio: false,
            tooltips: {
                backgroundColor: "rgb(255,255,255)",
                bodyFontColor: "#858796",
                borderColor: '#dddfeb',
                borderWidth: 1,
                xPadding: 15,
                yPadding: 15,
                displayColors: false,
                caretPadding: 10,
            },
            legend: {
                display: false
            },
            cutoutPercentage: 80,
        },
    });

});




