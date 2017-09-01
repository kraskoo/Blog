document.addEventListener("DOMContentLoaded", function() {
    var links = document.getElementsByTagName("a");
    var modalLinks = [];
    var i;
    for (i = 0; i < links.length; i++) {
        var dataToggle = links[i].getAttribute("data-toggle");
        if (dataToggle === "modal") {
            modalLinks.push(links[i]);
        }
    }

    function proceedRequest(data, replyId) {
        var replyIdInput = document.getElementById("reply-id");
        var replyTextArea = document.getElementById("reply-text");
        var userIdInput = document.getElementById("user-id");
        var topicIdInput = document.getElementById("topic-id");
        var replyDateInput = document.getElementById("reply-date");
        replyTextArea.value = data.Text;
        replyIdInput.setAttribute("value", replyId);
        userIdInput.setAttribute("value", data.ReplierId);
        topicIdInput.setAttribute("value", data.TopicId);
        replyDateInput.setAttribute("value", data.ReplyDateString);
    }

    for (i = 0; i < modalLinks.length; i++) {
        modalLinks[i].addEventListener("click", function(ev) {
            var target = ev.currentTarget;
            var replyId = target.getAttribute("reply-id");
             $.ajax({
                 dataType: "json",
                 contentType: "application/json",
                 url: ("/api/webcontent/reply?replyid=" + replyId)
             }).done(function(data) { proceedRequest(data, replyId); });
        }, false);
    }
});