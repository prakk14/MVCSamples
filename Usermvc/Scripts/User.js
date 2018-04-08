$(function () {


    //This is where the dropdownlist cascading main function
    $.ajax({
        type: "POST",
        url: "Country",                            //Your Action name in the DropDownListConstroller.cs
        data: "",  //Parameter in this function, Is cast sensitive and also type must be string
        contentType: "application/json; charset=utf-8",
        dataType: "json"

    }).done(function (data) {

        console.log(data);
        //When succeeded get data from server construct the dropdownlist here.
        if (data != null) {

            $('#CountryName').empty();
            $('#CountryName').append('<option value="0">--Select Country--</option>');

            $('#StateName').empty();
            $('#StateName').append('<option value="0">--Select State--</option>');

            $.each(data.lstCountryItem, function (index, data) {
                $('#CountryName').append('<option value="' + data.Value + '">' + data.Text + '</option>');
            });
        }
    }).fail(function (response) {
        if (response.status != 0) {
            alert(response.status + " " + response.statusText);
        }
    });

    $("#CountryName").change(function () {
        var CountryName = $('#CountryName :selected').val();
        CountryName = CountryName == "" ? 0 : CountryName;
        //When select 'optionLabel' we need to reset it to default as well. So not need 
        //travel back to server.
        if (CountryName == "") {
            $('#StateName').empty();
            $('#StateName').append('<option value="">--Select a State--</option>');
            return;
        }

        //This is where the dropdownlist cascading main function
        $.ajax({
            type: "POST",
            url: "State",                            //Your Action name in the DropDownListConstroller.cs
            data: "{'pCountry':'" + CountryName + "'}",  //Parameter in this function, Is cast sensitive and also type must be string
            contentType: "application/json; charset=utf-8",
            dataType: "json"

        }).done(function (data) {
            //When succeeded get data from server construct the dropdownlist here.
            if (data != null) {

                $('#StateName').empty();
                $('#StateName').append('<option value="0">--Select State--</option>');
                $.each(data.lstStateItem, function (index, data) {
                    $('#StateName').append('<option value="' + data.Value + '">' + data.Text + '</option>');
                });
            }
        }).fail(function (response) {
            if (response.status != 0) {
                alert(response.status + " " + response.statusText);
            }
        });

    });
});