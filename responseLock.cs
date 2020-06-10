using System;
using System.Collections.Generic;
using System.Text;

namespace kuroyukibot
{
    public class responseLock
    {
        public responseLock(){
            groupId = 0;
            discussId = 0;
            discussUserId = 0;
            groupUserId = 0;
        }
        public long groupUserId { get; set; }
        public long discussUserId { get; set; }
        public long groupId { get; set; }
        public long discussId { get; set; }

        public bool isGroupLocked()
        {
            return groupId == 0 ? false : true;
        }

        public bool isDiscussLocked()
        {
            return discussId == 0 ? false : true;
        }

        public string checkDiscussStatus()
        {
            return "正在等待来自" + discussUserId + "的图片，\n如要放弃搜索，请发送\"放弃\"";
        }

        public string checkGroupStatus()
        {
            return "正在等待来自" + groupUserId + "的图片，\n如要放弃搜索，请发送\"放弃\"";
        }
    }
}
