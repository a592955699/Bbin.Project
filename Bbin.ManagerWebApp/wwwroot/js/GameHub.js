"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();

connection.on("Send", function (message) {
    console.log("Send " + message);
});

connection.on("JoinGroup", function (groupName) {
    var encodedMsg = "已经加入组 " + groupName;
    console.log(encodedMsg);
});

connection.on("JoinGroups", function (groupName) {
    var encodedMsg = "已经加入组 " + groupName;
    console.log(encodedMsg);
});

connection.on("LeaveGroup", function (groupName) {
    var encodedMsg = "已经离开 " + groupName;
    console.log(encodedMsg);
});

connection.on("LeaveGroups", function (groupName) {
    var encodedMsg = "已经离开 " + groupName;
    console.log(encodedMsg);
});

connection.on("PushResult", function (data) {
    console.log(data);
    //room 大块
    var roomBlock = $("#" + getRoomIdGroupName(data.roomId));

    //清空推荐
    var recommendBlock = roomBlock.find(".recommend");
    recommendBlock.empty().hide();
    
    //标题
    roomBlock.find(".title .roomIndex").text(data.index);
    roomBlock.find(".title .roomDate").text(data.date);

    //数字结果
    var nubmerBlock = roomBlock.find(".number");
    nubmerBlock.empty();
    for (var i = 0; i < data.numberResults.length; i++) {
        var item = data.numberResults[i];
        nubmerBlock.append('<div class="cell state-' + item.resultState + '" title="顺序:' + item.index + '">' + (item.number === 0 ? "" : item.number) + '</div>');
    }

    //颜色结果
    var resultBlock = roomBlock.find(".result");
    resultBlock.empty();

    var columns = data.columnResults;
    //庄闲结果，只取最后12列
    var maxCol = 18;
    if (data.columnResults.length > maxCol) {
        columns = new Array();
        for (var i = data.columnResults.length - maxCol; i < data.columnResults.length; i++) {
            columns.push(data.columnResults[i]);
        }
    }

    for (var i = 0; i < columns.length; i++) {
        var col = columns[i];
        for (var j = 0; j < col.length; j++) {
            var item = col[j];
            resultBlock.append('<div class="cell state-' + item + '"></div>');
        }
        if (col.length % 6 !== 0) {
            var fill = 6 - col.length % 6;
            for (var f = 0; f < fill; f++) {
                resultBlock.append('<div class="cell state-0"></div>');
            }
        }
    }

    if (data.recommend !== null && data.recommend.length > 0) {
       
        console.log(data.recommend);
        var array = new Array();
        for (var i = 0; i < data.recommend.length; i++) {
            var item = data.recommend[i];
            array.push(item.name + " 推荐:(" + getRecommendTypeText(item.recommendType) + ")" + getResultStateText(item.resultState));
        }

        recommendBlock.html(array.join(",")).show();
        console.log(data.roomId + " " + data.roomName + " 推荐结果:" + array.join(","));
    }
});

connection.onclose(async () => {
    console.log("closed");
    setTimeout(() => start(), 5000);
});

//处理链接关闭情况，onclose监听服务器断开和客户端主动断开  ，try catch 监听服务器无法访问等
//在实际应用中，重新连接超过指定次数后放弃
async function start() {
    try {
        console.log("准备重连...");
        connection.start().then(function () {
            console.log("connected");
            joinGroup();
        }).catch(function (err) {
            return console.error(err.toString());
        });
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 1000);
    }
};

$(document).ready(async function () {
    await start();
    $('input[name=chk_roomId][type=checkbox]').click(function () {
        var roomId = $(this).val();
        if ($(this).is(':checked')) {
            $("#result_" + roomId).show();
            connection.invoke("JoinGroup", getRoomIdGroupName(roomId)).catch(function (err) {
                return console.error(err.toString());
            });
        }
        else {
            $("#result_" + roomId).hide();
            connection.invoke("LeaveGroup", getRoomIdGroupName(roomId)).catch(function (err) {
                return console.error(err.toString());
            });
        }
    });
});

function joinGroup() {
    //console.log("准备加入组")
    //var joinGroups = new Array();
    //var checkRooms = $('input[name=chk_roomId][type=checkbox]:checked');
    //checkRooms.each(function (i) {
    //    joinGroups.push(getRoomIdGroupName($(this).val()));
    //});
    //connection.invoke("JoinGroups", joinGroups).catch(function (err) {
    //    return console.log(err.toString());
    //});
}
function getRoomIdGroupName(roomId) {
    return "room_" + roomId;
}

function getResultStateText(resultState) {
    switch (resultState) {
        case 3:
            return "庄";
        case 2:
            return "和";
        case 1:
            return "闲";
        default:
            return "";
    }
}
function getRecommendTypeText(recommendType) {
    switch (recommendType) {
        case 0:
            return "跟";
        case 1:
            return "断";
        default:
            return "";
    }
}
