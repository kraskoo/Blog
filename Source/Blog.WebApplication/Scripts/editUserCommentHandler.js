document.addEventListener("DOMContentLoaded", function() {
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

    this.addEventListener("click", function (ev) {
        var target = ev.target;
        var replyId = target.getAttribute("reply-id");
        if (target.getAttribute("data-target") === "#edit") {
            $.ajax({
                dataType: "json",
                contentType: "application/json",
                url: ("/api/webcontent/reply?replyid=" + replyId)
            }).done(function (data) { proceedRequest(data, replyId); });
        }
    }, true);
});