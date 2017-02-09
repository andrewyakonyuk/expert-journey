var Data = $('#dataArticleList');
var PageCnt = $(Data).data('pagecnt');
var Type = $(Data).data('showbytags');
var LastId = $(Data).data('lastid');



var inProgress = false;
var startFrom = 1;
var userId = 0;
var onlyInterestingNews = false;


function RequestArticles() {
    $.ajax({
        url: '/News/GetArticles',
        //              timeout: 3000,
        method: 'POST',
        dataType: 'Json',
        async: true,
        data: { "page": startFrom, "lastId": LastId, "type": Type },
        beforeSend: function () {
            inProgress = true;
        }
    }).done(function (data) {
        if (data.length > 0) {
            $.each(data, function (index, data) {
                var templ = ($("#template").html().split("[Id]").join(data.Id));

                templ = templ.split("[Title]").join(data.Title);
                templ = templ.replace('[ShortDescription]', data.ShortDescription);
                templ = templ.split("[Date]").join(data.CreateDate.replace("T", " "));
                if (data.Image != 'Empty') {
                    var image = $('#imageTempl').html();
                    image = image.split('[Id]').join(data.Id);
                    image = image.split('[Image]').join(data.Image);
                    templ = templ.replace('[ImagePlaceholder]', image);
                }
                else {
                    templ = templ.replace('[ImagePlaceholder]', '');
                }
                var $temp = $(templ);
                $('#grid').append($temp).masonry('appended', $temp);
            });
            LastId = data[data.length - 1].Id;
        }
        startFrom++;
        inProgress = false;
    });
}

function CallAdaptive() {
    $('#grid').masonry({
        itemSelector: '.grid-item',
        singleMode: false,
        isResizable: true,
        isAnimated: true,
        animationOptions: {
            queue: false,
            duration: 500
        }
    });

}

$(document).ready(function () {
    CallAdaptive();
    $(window).scroll(function () {
        if (startFrom < PageCnt && !inProgress && $(window).scrollTop() >= $(document).height() - $(window).height() - 250) {
            RequestArticles();
        }
    });
});
