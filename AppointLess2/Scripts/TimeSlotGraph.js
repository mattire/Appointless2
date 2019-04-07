

function WriteToCanvas(hour, mins, days, color)
{
    var canvas = document.getElementById("schedMiniMap");
    var ctx = canvas.getContext("2d");
    ctx.fillStyle = color;
    //ctx.fillRect(0,0,150,75);
    i = 0;
    days.forEach(function (d)
    {
        //if (d === 1) {
        if (d == true) {
            var startX = i * 50;
            var lenX = 50;
            var startY = hour;
            var lenY = mins / 6;
            ctx.fillRect(startX, startY, 50, lenY);
        }
        i++;
    });
}

function Lines() {
    var c = document.getElementById("schedMiniMap");
    var ctx = c.getContext("2d");

    for (var i = 1; i < 7; i++) {
        ctx.beginPath();
        ctx.moveTo(i * 50, 0);
        ctx.lineTo(i * 50, 240);
        ctx.stroke();
    }

    var startElem = document.getElementById("schedStart");
    var endElem   = document.getElementById("schedEnd");

    if (startElem !== undefined && endElem !== undefined) {
        var dayStart = parseInt(startElem.textContent) * 10;
        var dayEnd = parseInt(endElem.textContent) * 10;

        ctx.beginPath();
        ctx.moveTo(0, dayStart );
        ctx.lineTo(350, dayStart);
        ctx.stroke();

        ctx.beginPath();
        ctx.moveTo(0, dayEnd);
        ctx.lineTo(350, dayEnd);
        ctx.stroke();
    }
}

function ReadWeekDays() {
    var Ma  = document.getElementById("Ma");
    var Ti  = document.getElementById("Ti");
    var Ke  = document.getElementById("Ke");
    var To  = document.getElementById("To");
    var Pe  = document.getElementById("Pe");
    var La  = document.getElementById("La");
    var Su = document.getElementById("Su");
   
    return [
        Ma.checked === true ? 1 : 0,
        Ti.checked === true ? 1 : 0,
        Ke.checked === true ? 1 : 0,
        To.checked === true ? 1 : 0,
        Pe.checked === true ? 1 : 0,
        La.checked === true ? 1 : 0,
        Su.checked === true ? 1 : 0,
    ];
}

function clearCanvas()
{
    var c = document.getElementById("schedMiniMap");
    var ctx = c.getContext("2d");
    ctx.clearRect(0, 0, c.width, c.height);
}

function ReadTimeAndLength()
{
    var tod = document.getElementById("TimeOfDay");
    var lm = document.getElementById("LengthMinutes");
   
    return {
        tod: tod.value,
        lm: lm.value,
    };
}

$('document').ready(function ()
{
    //try {
        inputChanged();
    //} catch (ex) {
    //    alert(ex);
    //}
});

function readTSMap()
{
    var tsLst = document.getElementsByClassName("TimeSlot");
    var arr = Array.prototype.slice.call(tsLst);
    var contentsLst = arr.map(function (a) { return a.textContent; });
    return contentsLst;
}

function binaryDowToBoolList(bVal) {
    return [
        (bVal & 1)  !== 0,
        (bVal & 2)  !== 0,
        (bVal & 4)  !== 0,
        (bVal & 8)  !== 0,
        (bVal & 16) !== 0,
        (bVal & 32) !== 0,
        (bVal & 64) !== 0,
    ];
}

var roundChange = false;

function roundInputs(h, m, mins)
{
    var lenMins = Math.round(mins / 15) * 15;
    var startMins = Math.round(m / 15) * 15;
    var newSTart = h + ":" + startMins;
    console.log(newSTart);
    roundChange = true;
    //$('#TimeOfDay').value = h + ":" + startMins;
    //$('#LengthMinutes').value = lenMins;
    var tod = document.getElementById("TimeOfDay");
    var lm = document.getElementById("LengthMinutes");
    tod.value = newSTart;
    lm.value = lenMins;
    roundChange = false;
}

function inputChanged()
{
    console.log("changed");
    if (roundChange) { return ; }
    clearCanvas();
        var {tod, lm } = ReadTimeAndLength();
    console.log(tod);

    var days = ReadWeekDays();
    var h = parseInt(tod.split(":")[0]);

    var m = parseInt(tod.split(":")[1]);
    var mins = parseInt(lm);

    roundInputs(h, m, mins);

    var y = Math.round((h + m / 60) * 10);
    WriteToCanvas(y, mins, days, "#FF000088");

    var tslLst = readTSMap();
    console.log(tslLst);

    tslLst.forEach(function (tsl) {
        var ints = tsl.split(",").map(function (s) { return parseInt(s); });
        var days = binaryDowToBoolList(ints[3]);
        var h = ints[0];
        var m = ints[1];
        var mins = ints[2];
        var y = Math.round((h + m / 60) * 10);
        WriteToCanvas(y, mins, days, "#0000FF88");
    });

    Lines();
}

$('#TimeOfDay')     .change(function () {inputChanged(); });
$('#LengthMinutes') .change(function () {inputChanged(); });

