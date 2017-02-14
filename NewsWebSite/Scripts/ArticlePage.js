connectionStarted = true;
var hub = $.connection.commentsHub;


var Data;
var myId;
var MaxCommentLength;
var ArticleId;;
var MyName;

var commentBlockLoaded = false;

var CommentsBlock;

var noComments = false;
$(document).ready(function () {
    CommentsBlock = $('#CommentsBlock')

    Data = $('#articledata');
    myId = $(Data).data('curuserid');
    MaxCommentLength = $(Data).data('maxlen');
    ArticleId = $(Data).data('articleid');
    MyName = $(Data).data('myname');
    $.connection.hub.start().done(function () {
        if ($(document).height() - 120 <= $(window).height() || $(window).scrollTop() >= $(document).height() - $(window).height() - 120) {
            LoadComments();
        }
        $(window).scroll(function () {
            if (!commentBlockLoaded && $(window).scrollTop() >= $(document).height() - $(window).height() - 120) {
                LoadComments();
            }
        });
        $('#comments').on("click", ".comment .contentComment", function () {

            var parent = $(this).parent();
            if (!$(parent).hasClass('green lighten-5')) {
                $('.comment.green.lighten-5').removeClass('green').removeClass('lighten-5');
            }
            $(parent).toggleClass('green lighten-5');
            // $(parent).addClass('green').addClass('lighten-5');
        });
    });

    $('#send').on("click", function () {
        var item = $('#message');
        var text = $(item).val().trim();
        var result = ValidateComment(text);
        if (result == "ok") {
            SendComment(0, text);
            $(item).val('');
            ResetDefaults();
        }
        else {
            var validationMsg = $('#sendBlock').find('.validComment:first');
            $(validationMsg).text(result).show();
            setTimeout(FadeOut, 5000, validationMsg);
            //Materialize.toast('Invalid input', 2000, 'red');
        }

    });
    $('#message').on("click", function () {
        ResetDefaults();
    });




    var CommentsBlock = $('#CommentsBlock');
    $(CommentsBlock).on('click', '.buttonsBlock .showSendBlock', function () {
        ResetDefaults();
        var buttons = $(this).parent();
        var comment = $(buttons).parent();
        //$(comment).find('.contentComment .staticCommentText:first').addClass('hidden');
        //$('.sendBlock.act').addClass('hidden').removeClass('act');
        $(buttons).find('.hidegroup').addClass('hidden').addClass('dis');
        $(comment).find('.buttonsBlock .sendBlock:first').removeClass('hidden').addClass('act');
        // $(this).addClass('hidden');
    });

    $(CommentsBlock).on('click', '.buttonsBlock .replyBtn', function () {
        var sendBlock = $(this).parent();
        var id = $(sendBlock).parent().attr('commentid');
        //id = id.replace('send-', '');
        var textArea = $(this).parent().find('.messageTextBox:first');
        var text = textArea.val().trim();
        var result = ValidateComment(text);
        if (result == "ok") {
            SendComment(id, text)
            // hub.server.send(ArticleId, id, text);
            $(textArea).val('');
            ResetDefaults();
        }
        else {
            var validationMsg = $(sendBlock).find('.validComment:first');
            $(validationMsg).text(result).show();
            setTimeout(FadeOut, 5000, validationMsg);
            //Materialize.toast('Invalid input', 2000, 'red');
        }
    });
    $(CommentsBlock).on('click', '.buttonsBlock .editButton', function () {
        ResetDefaults();
        $(this).addClass('hidden').addClass('dis');
        var commentsBlock = $(this).parent();
        $(commentsBlock).find('.hidegroup').addClass('hidden').addClass('dis');


        //$('#deleteBtn-' + id).addClass('hidden').addClass('dis');
        // $('#' + id).addClass('hidden').addClass('dis');
        var comment = $(commentsBlock).parent();
        var text = $(comment).find('.contentComment .staticCommentText:first').addClass('hidden').addClass('dis').text();
        // var text = $('#text-' + id).addClass('hidden').text();
        var edit = $(comment).find('.contentComment .editor:first');
        $(edit).text(text);
        $(edit).removeClass('hidden').addClass('act');
        $(comment).find('.buttonsBlock .saveButton:first').removeClass('hidden').addClass('act');
    });

    $(CommentsBlock).on('click', '.buttonsBlock .saveButton', function () {
        var comment = $(this).parent().parent();
        var contentComment = $(comment).find('.contentComment:first');
        var id = $(comment).attr('id');
        var value = $(contentComment).find('.commentText.editor:first').val().trim();
        var oldValue = $(contentComment).find('.staticCommentText:first');
        if (value == $(oldValue).text()) {
            ResetDefaults();
        }
        else {
            var result = ValidateComment(value);
            if (result == "ok") {
                $(oldValue).text(value);
                hub.server.edit(id, value);
                ResetDefaults();
            }
            else {
                var validationMsg = $(comment).find('.contentComment .validEdit:first');
                $(validationMsg).text(result).show();
                setTimeout(FadeOut, 5000, validationMsg);
                //Materialize.toast('Invalid input', 2000, 'red');
            }
        }
    });

    $(CommentsBlock).on('click', '.deleteButton', function () {
        var id = $(this).parent().attr('commentid');
        hub.server.delete(id);
        ResetDefaults();
    });



});


function LoadComments() {
    $.ajax({
        url: '/News/GetComments',
        //              timeout: 3000,
        method: 'POST',
        dataType: 'Json',
        async: true,
        data: { "articleId": ArticleId },
        beforeSend: function () {
            commentBlockLoaded = true;
            $('#loader').removeClass("hidden");
            hub.server.connect(ArticleId);
        }
    }).done(function (data) {
        //   console.log(data);
        if (data.length > 0) {
            $.each(data, function (index, data) {
                var templ = TemplateReplace(data.Id, data.UserId, data.UserName, data.Text, data.Created.replace('T', ' '));
                if (data.Depth == 0) {
                    $('#comments').prepend(templ);
                }
                else {

                    $('#' + data.ReplyCommentId).find('.replyBlock:first').prepend(templ);
                }
                if (data.Deleted) {
                    var comment = $('#' + data.Id)
                    $(comment).find('.buttonsBlock:first').addClass('hidden');
                    $(comment).find('.contentComment .staticCommentText:first').text('Comment has been deleted');
                }
                // LoadButtons(data.Id, data.UserId);
            });
        }

        $('#loaderBlock').addClass("hidden");
        $('#sendBlock').removeClass("hidden");
    });
}

hub.client.result = function (commentId, sendId, result, date) {
    var comment = $('#' + sendId);
    if (result == "ok") {
        $(comment).attr('id', commentId);
        $(comment).find('.commentDate:first').text(date);
        $(comment).find('.buttonsBlock:first').removeClass('hidden').attr('commentid', commentId);
        $(comment).find('.lineloaderplace:first').remove();
    }
    console.log(result);
}

hub.client.addMessage = function (id, userId, name, message, date, reply) {
    var templ = TemplateReplace(id, userId, name, message, date);
    if (reply == 0) {
        $('#comments').prepend(templ);
    }
    else {
        $('#reply-' + reply).prepend(templ);
    }
    // LoadButtons(id, userId);
}



hub.client.edit = function (commentId, text, date) {
    var commentContent = $('#' + commentId).find('.contentComment:first');
    $(commentContent).find('.staticCommentText:first').text(text);
    $(commentContent).find('.commentDate:first').text(date);
}
hub.client.delete = function (commentId) {
    var comment = $('#' + commentId);
    var commentcontent = $(comment).find('.contentComment:first');
    $(commentcontent).find('.staticCommentText:first').text('Comment has been deleted');
    $(commentcontent).find('.commentName:first').text('');
    $(comment).find('buttonsBlock:first').addClass('hidden');
    $(comment).addClass('deleted');
}

function SendComment(replyId, text) {
    var sendId = guid();
    hub.server.send(ArticleId, replyId, text, sendId);

    var templ = TemplateReplace(sendId, myId, MyName, text, "", replyId);
    templ = $(templ);
    if (replyId == 0) {
        $('#comments').prepend(templ);
    }
    else {
        $('#' + replyId).find('.replyBlock:first').prepend(templ);
    }

    $(templ).addClass('loading');
    var progressLine = $('<div />').addClass("progress").append($('<div />').addClass('indeterminate'));
    $(templ).find('.lineloaderplace:first').append($('<div />').addClass('progressline').append(progressLine));
    $(templ).find('.buttonsBlock:first').addClass('hidden');
}


function ValidateComment(text) {
    if (text.length > MaxCommentLength) {
        return "The max length of the comment is " + MaxCommentLength;
    }
    else if (text.length == 0) {
        return "Comment can not be empty";
    }
    return "ok";
}

function TemplateReplace(id, userId, name, message, date) {
    var templ = $("#template").html();
    templ = templ.split('[Id]').join(id);
    templ = templ.split('[userId]').join(userId);
    templ = templ.replace('[Name]', htmlEncode(name));
    templ = templ.replace('[Text]', htmlEncode(message));
    templ = templ.replace('[Date]', date.replace('T', ' '));
    if (userId == myId) {
        templ = templ.split('[Class]').join('commentsBtn');
    }
    else {
        templ = templ.split('[Class]').join('hidden');
    }
    return templ;
}



function ResetDefaults() {
    $('.editor.act').addClass('hidden').removeClass('act');
    $('.staticCommentText.hidden').removeClass('hidden');
    $('.saveButton.act').addClass('hidden').removeClass('act');
    $('.showSendBlock.hidden').removeClass('hidden');
    $('.sendBlock.act').addClass('hidden').removeClass('act');
    $('.editButton.hidden.dis').removeClass('hidden').removeClass('dis');
    $('.deleteButton.hidden.dis').removeClass('hidden').removeClass('dis');
    $('.sendBlock.act').addClass('hidden').removeClass('act');
    $('.showSendBlock.hidden').removeClass('hidden');
}


//--------------- Move to document ready


//--------------

//function LoadButtons(id, userId) {
//    $('#' + id).on("click", function () {
//        ResetDefaults();
//        var replyBtnId = $(this).attr('id');
//        var sendBlock = $('.sendBlock.act');
//        sendBlock.addClass('hidden');
//        sendBlock.removeClass('act');
//        $('#sendBlock-' + replyBtnId).addClass('act').removeClass('hidden');
//        // $('.showSendBlock.hidden').removeClass('hidden');
//        $(/*'#' + replyBtnId*/this).addClass('hidden');
//    });

//    $('#send-' + id).on("click", function () {
//        var id = $(this).attr('id');
//        id = id.replace('send-', '');
//        var textArea = $('#message-' + id);
//        var text = textArea.val().trim();
//        if (ValidateComment(text)) {
//            SendComment(id, text)


//            hub.server.send(ArticleId, id, text);
//            $(textArea).val('');
//            ResetDefaults();
//        }
//        else {
//            Materialize.toast('Invalid input', 2000, 'red');
//        }
//    });

//    if (userId == myId) {
//        $('#editBtn-' + id).on("click", function () {
//            ResetDefaults();
//            $(this).addClass('hidden').addClass('dis');
//            $('#deleteBtn-' + id).addClass('hidden').addClass('dis');
//            $('#' + id).addClass('hidden').addClass('dis');
//            var text = $('#text-' + id).addClass('hidden').text();
//            var edit = $('#edit-' + id);
//            $(edit).text(text);
//            $(edit).removeClass('hidden').addClass('act');
//            $('#SaveBtn-' + id).removeClass('hidden').addClass('act');
//        });

//        $('#SaveBtn-' + id).on("click", function () {
//            var value = $('#edit-' + id).val().trim();
//            if (ValidateComment(value)) {
//                hub.server.edit(id, value);
//                ResetDefaults();
//            }
//            else {
//                Materialize.toast('Invalid input', 2000, 'red');
//            }
//        });

//        $('#deleteBtn-' + id).on("click", function () {
//            ResetDefaults();
//            hub.server.delete(id);
//        });
//    }
//}

function guid() {
    return s4() + s4() + s4();
}

function s4() {
    return Math.floor((1 + Math.random()) * 0x10000)
      .toString(16)
      .substring(1);
}

function FadeOut(item) {
    $(item).fadeOut(1000, function () {
        $(item).hide();
    });
}