
if (!connectionStrted) {
    var hub = $.connection.commentsHub;
    connectionStarted = true;
    $.connection.hub.start();
}

hub.client.notify = function (commentId, fromWho, message, articleId) {
    var cntItem = $('#nCnt');
    var number = parseInt($(cntItem).text(), 10);
    number = number + 1;
    cntItem.text(number);
    Materialize.toast(fromWho + ': ' + htmlEncode(message), 5000);
}