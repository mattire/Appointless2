

var TimeSlots = [];

function DaysToNums(days) {
    var nums = [
        (days & 1) ? 1 : -1,
        (days & 2) ? 2 : -1,
        (days & 4) ? 3 : -1,
        (days & 8) ? 4 : -1,
        (days & 16) ? 5 : -1,
        (days & 32) ? 6 : -1,
        (days & 64) ? 7 : -1,
    ];
    return nums.filter(function (v) { return v != -1 });
}

function range(start, end, step = 1) {
    const len = Math.floor((end - start) / step) + 1
    return Array(len).fill().map((_, idx) => start + (idx * step))
}

function ToHourMins(hour, min, len) {
    var decStart = parseInt(hour) + parseInt(min) / 60;
    var decDiff = len / 60;
    var decs = range(decStart, decStart + decDiff, 0.25);
    var arr = decs.map(function (d) { var hours = Math.floor(d); mins = (d - hours) * 60; return hours + ":" + MinsToString(mins); });
    arr.pop();
    return arr;
}

function ToDtBlocks(hourMins, dnums) {
    return dnums.map(function (d) {
        return hourMins.map(function (hm) {
            return "td" + d + "-" + hm;
        });
    });
}

function MinsToString(min) {
    return (min + "").length == 1 ? "0" + min : "" + min;
}

function WriteDTBlock(dtBlock, color, tsId, allowClick) {
    //var borderStyle = "thin solid #000000";
    var borderStyle = "2px solid #000000";
    //console.log(dtBlock);
    var dtElems = dtBlock.map(function (dt) { return document.getElementById(dt); });
    var arr = Array.prototype.slice.call(dtElems);
    var first = arr[0];
    var last = arr[arr.length - 1];

    first.style.borderTop = borderStyle;
    last.style.borderBottom = borderStyle;
    arr.forEach(function (elem) {
        elem.style.borderLeft = borderStyle;
        elem.style.borderRight = borderStyle;
        elem.style.backgroundColor = color;
        //elem.dataset["timeSlot"] = tsId;
        if (allowClick) {
            elem.addEventListener("click", function () { HandleClick(tsId, elem); }, false);
        }

    });
}

function DayNumFromDtStr(tdStr) {
    return tdStr.slice(2, 3);
}

function WriteTsToTable(tsStr, currentDay, pholidays) {
    //tsStr = tsStr.replace(/\r?\n|\r/g, '');
    console.log(tsStr);
    var spl = tsStr.split(",");

    var hour = spl[0];
    var min = spl[1];
    var lenMins = spl[2];
    var books = spl[3];
    var conBooks = spl[4];
    var days = spl[5];
    var tsId = spl[6];

    var dnums = DaysToNums(days);
    var dbooks = DaysToNums(books);
    var dcbooks = DaysToNums(conBooks);
    var hourMins = ToHourMins(hour, min, lenMins);
    var dtBlocks = ToDtBlocks(hourMins, dnums) //, dbooks, dcbooks)

    for (var i in dtBlocks) {
        var dtBlock = dtBlocks[i];

        var dayNum = parseInt(DayNumFromDtStr(dtBlock[0]));

        console.log("===================");
        console.log(dayNum);
        console.log(currentDay);
        console.log(pholidays);
        //if (dayNum > currentDay && !(dayNum in pholidays))
        if (dayNum > currentDay && $.inArray(dayNum, pholidays) == -1) {

            if (!dbooks.includes(dayNum)) {
                WriteDTBlock(dtBlock, '#77ccff', tsId, true);
            }
            else {
                if (!dcbooks.includes(dayNum)) {
                    WriteDTBlock(dtBlock, '#ffcc77', tsId, false);
                } else {
                    WriteDTBlock(dtBlock, '#ff7711', tsId, false);
                }
            }
        }
    }

}

var WeekTimeSlots = new Array();

function ToMonthAndDay(dayOfWeek) {
    //console.log(dayOfWeek);
    dayOfWeek = dayOfWeek == 7 ? 0 : dayOfWeek;
    var key = 'th-' + dayOfWeek;
    var th = document.getElementById(key);

    return [th.innerText, th.dataset.dateText];
}

function HandleClick(tsId, elem) {
    var day = DayNumFromDtStr(elem.id);
    var pair = WeekTimeSlots.find(function (p) { return p.key == tsId; });
    var spl = pair.value;
    var time = spl[0] + ":" + MinsToString(spl[1]) + " len: " + spl[2];
    //var evInpWidth = $('#EventInput').clientWidth;

    var time = spl[0] + ":" + MinsToString(spl[1]);
    var dayOfWeek = parseInt(elem.id.split('-')[0].slice(2));
    var tup = ToMonthAndDay(dayOfWeek);
    var md = tup[0];
    var dateString = tup[1];

    $("#dateAndTime").text(md + " " + time + "\n" + spl[2] + "minuuttia");
    var elemSlotId = document.getElementById("Booking_TimeSlotId");
    var bookingED  = document.getElementById("Booking_EventDate");
    elemSlotId.value = tsId;
    bookingED.value = dateString;

    $('#EventInput').show();
    var eiWidth = $('#EventInput').width();
    eiWidth = eiWidth > 400 ? 300 : eiWidth;
    $("#EventInput").css({ top: event.clientY + 30, left: event.clientX - eiWidth / 2, position: 'absolute' });
}

function HideInput() {
    //$('#EventInput').style.display = 'none';
    $('#EventInput').hide();
}

function InitWeekTimeSlots(contentsLst) {
    var splits = contentsLst.map(function (c) { return c.split(","); });
    for (var i in splits) {
        var spl = splits[i];
        WeekTimeSlots.push({ key: spl[6], value: spl });
    }
    //console.log("**");
    //console.log(WeekTimeSlots);
}

function ToDict(strArray) {
    var d = {};
    strArray.map(function (str) {
        var spl = str.split(',');
        d[spl[0]] = spl[1];
    });
    return d;
}

function WriteHolidaysToTable(holidayNums) {
    for (var i = 0; i < holidayNums.length; i++) {
        var hn = holidayNums[i];
        var select = "[id*='td" + hn + "-']";
        var elems = document.querySelectorAll(select);
        for (var i2 in elems) {
            var e = elems[i2];
            if (e.style !== undefined) {
                e.style.backgroundColor = '#ff0000';
            }
        }
    }
}
