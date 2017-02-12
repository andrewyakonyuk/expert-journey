connectionStarted = true;
var hub = $.connection.commentsHub;


var Data = $('#articledata');
var myId = $(Data).data('userid');
var MaxCommentLength = $(Data).data('maxlen');
var ArticleId = $(Data).data('articleid');
var commentBlockLoaded = false;

var noComments = false;
$(document).ready(function () {
    $.connection.hub.start().done(function () {
        if ($(document).height() - 150 <= $(window).height() || $(window).scrollTop() >= $(document).height() - $(window).height() - 150) {
            LoadComments();
        }
        $(window).scroll(function () {
            if (!commentBlockLoaded && $(window).scrollTop() >= $(document).height() - $(window).height() - 150) {
                LoadComments();
            }
        });
        $('#comments').on("click", ".contentComment", function () {
            $('.comment.green.lighten-5').removeClass('green').removeClass('lighten-5');
            var parent = $(this).parent();
            $(parent).addClass('green').addClass('lighten-5');
        });
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
                    $('#comments').append(templ);
                }
                else {
                    $('#comment-' + data.ReplyCommentId).append(templ);
                }
                if (data.Deleted) {
                    $('#buttons-' + data.Id).addClass('hidden');
                    $('#text-' + data.Id).text('Comment has been deleted');
                }
                LoadButtons(data.Id, data.UserId);
            });
        }

        $('#loaderBlock').addClass("hidden");

        $('#send').on("click", function () {
            var item = $('#message');
            var text = $(item).val().trim();

            if (ValidateComment(text)) {
                hub.server.send(ArticleId, 0, text);
                $(item).val('');
                ResetDefaults();
            }
            else {
                Materialize.toast('Invalid input', 2000, 'red');
            }

        });
        $('#message').on("click", function () {
            $('.sendBlock.act').addClass('hidden').removeClass('act');
            $('.showSendBlock.hidden').removeClass('hidden');
        });

        $('#sendBlock').removeClass("hidden");
    });
}

hub.client.addMessage = function (id, userId, name, message, date, reply) {
    var templ = TemplateReplace(id, userId, name, message, date);
    if (reply == 0) {
        $('#comments').append(templ);
    }
    else {
        $('#comment-' + reply).append(templ);
    }
    LoadButtons(id, userId);
}

hub.client.edit = function (commentId, text, date) {
    $('#text-' + commentId).text(text);
    $('#commentDate-' + commentId).text(date);
}
hub.client.delete = function (commentId) {
    $('#text-' + commentId).text('Comment has been deleted');
    $('#commentName-' + commentId).text('');
    $('#buttons-' + commentId).addClass('hidden');
    $('#comment-' + commentId).addClass('deleted');
}

function ValidateComment(text) {
    if (text.length > MaxCommentLength || text.length == 0) {
        return false;
    }
    return true;
}

function TemplateReplace(id, userId, name, message, date, reply) {
    var templ = $("#template").html();
    templ = templ.split('[Id]').join(id);
    templ = templ.replace('[uId]', userId);
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
    $('.saveButton.act').addClass('hidden').removeClass('act');
    $('.commentContent.hidden').removeClass('hidden');
    $('.showSendBlock.hidden').removeClass('hidden');
    $('.sendBlock.act').addClass('hidden').removeClass('act');
    $('.editButton.hidden.dis').removeClass('hidden').removeClass('dis');
    $('.deleteButton.hidden.dis').removeClass('hidden').removeClass('dis');
    $('.sendBlock.act').addClass('hidden').removeClass('act');
    $('.showSendBlock.hidden').removeClass('hidden');
}

function LoadButtons(id, userId) {
    $('#' + id).on("click", function () {
        ResetDefaults();
        var replyBtnId = $(this).attr('id');
        var sendBlock = $('.sendBlock.act');
        sendBlock.addClass('hidden');
        sendBlock.removeClass('act');
        $('#sendBlock-' + replyBtnId).addClass('act').removeClass('hidden');
        $('.showSendBlock.hidden').removeClass('hidden');
        $('#' + replyBtnId).addClass('hidden');
    });

    $('#send-' + id).on("click", function () {
        var id = $(this).attr('id');
        id = id.replace('send-', '');
        var textArea = $('#message-' + id);
        var text = textArea.val().trim();
        if (ValidateComment(text)) {
            hub.server.send(ArticleId, id, text);
            $(textArea).val('');
            ResetDefaults();
        }
        else {
            Materialize.toast('Invalid input', 2000, 'red');
        }
    });

    if (userId == myId) {
        $('#editBtn-' + id).on("click", function () {
            ResetDefaults();
            $(this).addClass('hidden').addClass('dis');
            $('#deleteBtn-' + id).addClass('hidden').addClass('dis');
            $('#' + id).addClass('hidden').addClass('dis');
            var text = $('#text-' + id).addClass('hidden').text();
            var edit = $('#edit-' + id);
            $(edit).text(text);
            $(edit).removeClass('hidden').addClass('act');
            $('#SaveBtn-' + id).removeClass('hidden').addClass('act');
        });

        $('#SaveBtn-' + id).on("click", function () {
            var value = $('#edit-' + id).text().trim();
            if (ValidateComment(value)) {
                hub.server.edit(id, value);
                ResetDefaults();
            }
            else {
                Materialize.toast('Invalid input', 2000, 'red');
            }
        });

        $('#deleteBtn-' + id).on("click", function () {
            ResetDefaults();
            hub.server.delete(id);
        });
    }
}