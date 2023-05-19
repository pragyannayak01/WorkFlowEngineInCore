//================common.js is common for all the pages. Common function will be written here===================
//to get the value from the query string
function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}
// To convert any date format to dd-MMM-yyyy
Date.prototype.toShortFormat = function () {

    let monthNames = ["Jan", "Feb", "Mar", "Apr",
        "May", "Jun", "Jul", "Aug",
        "Sep", "Oct", "Nov", "Dec"];

    let day = this.getDate();

    let monthIndex = this.getMonth();
    let monthName = monthNames[monthIndex];

    let year = this.getFullYear();
    if (day.toString().length == 1) {
        return `${"0" + day}-${monthName}-${year}`;
    }
    else {
        return `${day}-${monthName}-${year}`;
    }
}
$('.numeric').on('input', function (event) {
    this.value = this.value.replace(/[^0-9]/g, '');
});
