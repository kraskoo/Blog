window.addEventListener("DOMContentLoaded", function () {
    var commentAccepter = $("#comment-accepter");
    $.ajax({
        url: ("/api/webcontent/comments?topicId=" + commentAccepter.attr("topic-id")),
        method: "GET",
        contentType: "application/json"
    }).done(processComments);
    function processComments(data) {
        var keys = Object.keys(data);
        for (var key in keys) {
            if (data.hasOwnProperty(key)) {
                var matched = ((data[key]["ReplyDate"])).toString().match(/\d+/);
                var date = new Date(Number(matched[0]));
                var formattedDate = date.getDay() + "/" + date.getMonth() + "/" + date.getFullYear();
                var userInfo = [];
                userInfo.push("<div class='media' user-id='" + data[key]["ReplierId"] + "' reply-id='" + data[key]["Id"] + "'>");
                userInfo.push("<div class='media-left'>");
                userInfo.push("<img src='#' class='media-object media-profile' alt='Cannot load image' />");
                userInfo.push("</div>");
                userInfo.push("<div class='media-body'>");
                userInfo.push("<h4 class='media-heading'><a href='/" + data[key]["ReplierId"] + "/comments/' class='text-danger'>" + data[key]["ReplierUserName"] + "</a> <small class='text-muted'>Posted on: " + formattedDate + "</small></h4>");
                userInfo.push("<p>" + data[key]["ReplyText"] + "</p>");
                userInfo.push("</div>");
                userInfo.push("</div>");
                $($.parseHTML(userInfo.join(""))).appendTo(commentAccepter);
                $.ajax({
                    url: ("/api/webcontent/iscurrentuserwatchtopicpage?userId=" + data[key]["ReplierId"] + "&replyId=" + data[key]["Id"] + "&key=" + key),
                    method: "GET",
                    contentType: "application/json"
                }).done(function(topicUserData) {
                    var isSameUser = topicUserData["IsSameUser"];
                    var replyId = topicUserData["ReplyId"];
                    var currentKey = topicUserData["Key"];
                    if (isSameUser) {
                        var replyText = data[currentKey]["ReplyText"];
                        var contentForm = [];
                        var divWithReplyId = $("div[reply-id]");
                        for (var i = 0; i < divWithReplyId.length; i++) {
                            var current = $(divWithReplyId[i]);
                            if (Number(current.attr("reply-id")) === replyId) {
                                contentForm.push("<div class='media-right'>");
                                contentForm.push("<div class='btn-group btn-group-justified'>");
                                contentForm.push("<a href='#' class='btn btn-default btn-sm' reply-id='" + replyId + "' data-toggle='modal' data-target='#edit' style='width: 43px'>Edit</a>");
                                contentForm.push("<a href='/reply/deletereply?replyId=" + replyId + "' class='btn btn-default btn-sm' style='width: 57px'>Delete</a>");
                                contentForm.push("</div>");
                                contentForm.push("</div>");
                                contentForm.push("<div class='modal fade' id='edit' role='dialog'>");
                                contentForm.push("<div class='modal-dialog'>");
                                contentForm.push("<div class='modal-content'>");
                                contentForm.push("<div class='modal-header'>");
                                contentForm.push("<button type='button' class='close' data-dismiss='modal'>&times;</button>");
                                contentForm.push("<h4 class='modal-title'></h4>");
                                contentForm.push("</div>");
                                contentForm.push("<div class='modal-body'>");
                                contentForm.push("<div class='row well'>");
                                contentForm.push("<form action='/reply/editinmodal' method='POST'>");
                                contentForm.push("<div class='form-group'>");
                                contentForm.push("<textarea id='reply-text' name='ReplayText' class='form-control' rows='5'>" + replyText + "</textarea>");
                                contentForm.push("</div>");
                                contentForm.push("<div class='form-group'>");
                                contentForm.push("<input type='text' id='user-id' name='UserId' class='form-control hidden' />");
                                contentForm.push("</div>");
                                contentForm.push("<div class='form-group'>");
                                contentForm.push("<input type='text' id='user-id' name='UserId' class='form-control hidden' />");
                                contentForm.push("</div>");
                                contentForm.push("<div class='form-group'>");
                                contentForm.push("<input type='text' id='reply-id' name='Id' class='form-control hidden' />");
                                contentForm.push("</div>");
                                contentForm.push("<div class='form-group'>");
                                contentForm.push("<input type='text' id='topic-id' name='TopicId' class='form-control hidden' />");
                                contentForm.push("</div>");
                                contentForm.push("<div class='form-group'>");
                                contentForm.push("<input type='text' id='reply-date' name='ReplayDate' class='form-control hidden' />");
                                contentForm.push("</div>");
                                contentForm.push("<div class='form-group'>");
                                contentForm.push("<input type='submit' value='Done' class='btn btn-default' />");
                                contentForm.push("</form>");
                                contentForm.push("</div>");
                                contentForm.push("</div>");
                                contentForm.push("</div>");
                                contentForm.push("</div>");
                                contentForm.push("</div>");
                                contentForm.push("</div>");
                                $($.parseHTML(contentForm.join(""))).appendTo(current[0]);
                            }
                        }
                    }
                });
                $.ajax({
                    url: ("/api/webcontent/userprofileimage?userId=" + data[key]["ReplierId"]),
                    method: "GET",
                    contentType: "application/json"
                }).done(function (imgData) {
                    var divWithUserInfoId = $("div[user-id]");
                    for (var i = 0; i < divWithUserInfoId.length; i++) {
                        var current = $(divWithUserInfoId[i]);
                        var currentImg = $(current).find(".media-left > img");
                        if (current.attr("user-id") === imgData["UserId"]) {
                            currentImg.attr("src", imgData["Img"]);
                        }
                    }
                });
            }
        }
    }
});