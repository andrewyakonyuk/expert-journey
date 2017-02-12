if (!connectionStrted) {
    var hub = $.connection.commentsHub;
    connectionStarted = true;
    $.connection.hub.start();
}

hub.client.notify = function (commentId, fromWho, message, articleId) {
    $('.nCnt').each(function (index, item) {
        var number = parseInt($(item).text(), 10);
        number++;
        $(item).text(number);
    });
    Materialize.toast(fromWho + ': ' + htmlEncode(message), 5000);
}