function HideInput(hashInp) {
    $(hashInp).hide();
}

function DisableBtn(elemId) {
    //console.log(elemId);
    $('#' + elemId)[0].disabled = true;
}