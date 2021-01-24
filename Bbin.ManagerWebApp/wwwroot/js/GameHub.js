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
    //room 大块
    var roomBlock = $("#" + getRoomName(data.RoomId));

    //清空推荐
    roomBlock.find(".recommend").empty();
    
    //标题
    roomBlock.find(".title .roomIndex").text(data.Index);
    roomBlock.find(".title .roomDate").text(data.Date);

    //数字结果
    var nubmerBlock = roomBlock.find("number");
    data.NumberResults.each(function (i) {
        var item = $(this);
        nubmerBlock.append('<div class="cell state-' + item.ResultState+'" title="顺序:' + item.Index + '">' + item.Number+'</div>');
    });

    //颜色结果
    var resultBlock = roomBlock.find("result");
    data.ColumnResults.each(function (colIndex) {
        var col = $(this);
        col.each(function (cellIndex) {
            var item = $(this);
            resultBlock.append('<div class="cell state-' + item.ResultState + '">' + item.Number + '</div>');
        });

        if (col.length % 6 != 0) {
            var fill = 6 - col.length % 6;
            for (var f = 0; f < fill; f++) {
                resultBlock.append('<div class="cell state-0"></div>');
            }            
        }
    });

    if (data.Recommend !== null && data.Recommend.length > 0) {
        var array = new Array();
        data.Recommend.each(function (i) {
            array.push($(this).Key.Name)
        });
        roomBlock.find(".recommend").html(array.join(","));
    }
});

connection.start().then(function () {
    joinGroup();
}).catch(function (err) {
    return console.error(err.toString());
});

$(document).ready(function () {
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
    var joinGroups = new Array();
    var checkRooms = $('input[name=chk_roomId][type=checkbox]:checked');
    checkRooms.each(function (i) {
        joinGroups.push(getRoomIdGroupName($(this).val()));
    });
    connection.invoke("JoinGroups", joinGroups).catch(function (err) {
        return console.error(err.toString());
    });
}


function getRooms() {
    return roomTables;
}

function getRoomName(roomId) {
    var rooms = getRooms();
    return rooms[roomId] === null ? roomId : rooms[roomId];
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
