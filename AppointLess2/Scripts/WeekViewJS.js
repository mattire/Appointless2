
var Mode = Object.freeze({ "admin": 1, "client": 2 });

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

function MinsToString(min) {
    return (min + "").length == 1 ? "0" + min : "" + min;
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

function ToMonthAndDay(dayOfWeek) {
    //console.log(dayOfWeek);
    dayOfWeek = dayOfWeek == 7 ? 0 : dayOfWeek;
    var key = 'th-' + dayOfWeek;
    var th = document.getElementById(key);

    return [th.innerText, th.dataset.dateText];
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


// 
// class TableSlot
// 
function TableSlot(startHour, startMins, lengthMins, days,
    bookings   = null,
    mode       = Mode.client,
    tsId       = null,
    currentDay = null,
    pholidays = null)
{
    this.mStartHour  = startHour;
    this.mStartMins  = startMins;
    this.mLengthMins = lengthMins;
    this.mDays       = days;
    this.mCellCount  = lengthMins / 15;
    this.mBookings   = bookings;
    this.mMode       = mode;
    this.mTimeSlotId = tsId;
    this.mCurrentDay = currentDay;
    this.mPHolidays  = pholidays;
}

TableSlot.prototype.print = function () {
    console.log(this.mStartHour);
    console.log(this.mStartMins);
    console.log(this.mLengthMins);
    console.log(this.mDays);
    console.log('--');
}

TableSlot.prototype.TdBlock2Elems = function (tdLst) {
    var elemLst = [];
    for (var i in tdLst) {
        elemLst.push(document.getElementById(tdLst[i]));
    }
    return elemLst;
}

TableSlot.prototype.DrawTdElemBlock = function (tdElemLst, borderStyle, bgColor, booking = null) {
    var first = tdElemLst[0];
    var last = tdElemLst[tdElemLst.length - 1];
    first.style.borderTop = borderStyle;
    last.style.borderBottom = borderStyle;

    if (booking != null) {
        first.textContent = booking.Email.length > 20 ? booking.Email.substr(0, 20) : booking.Email;
    }

    for (var i in tdElemLst) {
        var e = tdElemLst[i];
        e.style.borderLeft = borderStyle;
        e.style.borderRight = borderStyle;
        e.style.backgroundColor = bgColor;
    }
}

TableSlot.prototype.SetEventHandler = function (eventStr, tdElemLst, clickHandler) {
    for (var i in tdElemLst) {
        var e = tdElemLst[i];
        //e.addEventListener('click', clickHandler);
        e.addEventListener(eventStr, clickHandler);
    }
}

TableSlot.prototype.WriteToTable = function () {
    var days = DaysToNums(this.mDays);
    var self = this;
    var hourMins = ToHourMins(this.mStartHour, this.mStartMins, this.mLengthMins);

    var dayTDBlocks = days.map(function (d) {
        var prefix = "td" + d;
        return { day: d, hourMins: hourMins.map(function (hm) { return prefix + "-" + hm; }) };
    });

    var elemBlocks = dayTDBlocks.map(function (d_td) { return { day: d_td.day, elemBlock: self.TdBlock2Elems(d_td.hourMins) }; });

    var bookedDays = this.mBookings.map(function (b) { return b.Day; });
    var bookingMap = this.mBookings.map(function (b) { return { day: b.Day, booking: b } });
    var self = this;
    elemBlocks.map(function (deb) {
        if ($.inArray(deb.day, bookedDays) == -1) // true if not in booked days
        {
            if (currentDay == null || currentDay < deb.day)
            {
                if ($.inArray(deb.day, self.mPHolidays) == -1) // skip holidays
                {
                    //if (deb.day > this.currentDay)
                    self.DrawTdElemBlock(deb.elemBlock, '2px solid #000000', '#77ccff');
                    //self.SetEventHandler(deb.elemBlock, function () { alert('afaf'); });
                    self.SetEventHandler('click', deb.elemBlock, function () {
                        self.UnreservedClickHandler(deb.elemBlock, event);
                    });
                }
            }
        } else {
            if (self.mMode == Mode.admin) {
                try {
                    var tup = bookingMap.find(function (t) { return t.day == deb.day; });
                    //var booking = bookingMap[deb.day];
                    var color = '#ffdd77';
                    if (tup.booking.Status >= 1) {
                        color = '#ddff44';
                    }
                    self.DrawTdElemBlock(deb.elemBlock, '2px solid #000000', color, tup.booking);
                    //console.log(tup.booking);
                    self.SetEventHandler('click', deb.elemBlock, function () {
                        self.AdminClickHandler(tup.booking, event);
                    });

                    //self.SetEventHandler('mouseover', deb.elemBlock, function () {
                    self.SetEventHandler('mouseenter', deb.elemBlock, function () {
                        self.AdminHoverHandler(tup.booking, event, true);
                    });
                    self.SetEventHandler('mouseleave', deb.elemBlock, function () {
                        self.AdminHoverHandler(tup.booking, event, false);
                    });
                } catch (e) {
                    console.log(e);
                }
            }
            if (self.mMode == Mode.client) {
                try {
                    self.DrawTdElemBlock(deb.elemBlock, '2px solid #000000', '#ff8855');
                } catch (e) {
                    console.log(e);
                }
            }
        }
    });
}

TableSlot.prototype.UnreservedClickHandler = function (elemBllock, ev) {
    console.log(ev.srcElement.id);
    var weekdayNum = ev.srcElement.id[2]
    var mon_n_day = ToMonthAndDay(weekdayNum);
    console.log(mon_n_day);
    $('#EventInput').show();
    console.log(elemBllock);
    $('#dateAndTime').text(
        mon_n_day[0] + " " +
        this.mStartHour + ":" + this.mStartMins + "\n" +
        this.mLengthMins + "minuuttia");

    //this.m
    $('#Booking_TimeSlotId').val(this.mTimeSlotId);
    $('#Booking_EventDate').val(mon_n_day[1]);
    
    //var firstElem = elemBllock[0];
    var eiWidth = $('#EventInput').width();
    eiWidth = eiWidth > 400 ? 300 : eiWidth;
    //this.mLengthMins
    $("#EventInput").css({ top: ev.clientY + 30, left: ev.clientX - eiWidth / 2, position: 'absolute' });
}

TableSlot.prototype.AdminClickHandler = function (booking, ev) {
    //console.log('booking.Phone');
    //console.log(booking.Phone);
    console.log(this.mLengthMins);
    console.log(booking.Name);
    console.log(booking.Email);
    console.log(booking.Description);
    

    $('#AdminBookingDataEName').text(booking.Name)
    $('#AdminBookingDataEmail').text(booking.Email)
    //$('#AdminBookingDataPhone').text(booking.)
    $('#AdminBookingDataDescription').text(
        booking.Description == null ? "" :
            booking.Description.length < 15 ?
                booking.Description : booking.Description.substr(0, 15));
    $('#AdminBookingDataStatus').text(booking.Status == 1 ? "Vahvistettu" : "Vahvistamaton");
    $('#bookingID').val(booking.ID + "");


    //$('#adminEmail'      ).val("" + this.mLengthMins);
    //$('#adminPhone'      ).val(booking.Name         );
    //$('#adminDescription').val(booking.Email);



    this.ShowOnClickPos('#AdminBookingView', ev);
}

TableSlot.prototype.AdminHoverHandler = function (booking, ev, enterleave) {
    if (enterleave == true) {
        var str = booking.Name + " " + booking.Email + " " + this.mLengthMins + " minuuttia";
        $('#adminHoverData').text(str);
        var offset = [-100, -10];
        this.ShowOnClickPos('#AdminHoverBookingView', ev, offset);
    } else {
        console.log('hide');
        $('#AdminHoverBookingView').hide();
    }

}

TableSlot.prototype.ShowOnClickPos = function (uiElem, ev, offset = null) {
    $(uiElem).show();
    var eiWidth = $(uiElem).width();
    eiWidth = eiWidth > 400 ? 300 : eiWidth;
    //this.mLengthMins
    if (offset == null) {
        offset = [0, 0];
    }
    $(uiElem).css({ top: ev.clientY + 30 + offset[0], left: ev.clientX - eiWidth / 2 + offset[1], position: 'absolute' });
}




var slotList = [];