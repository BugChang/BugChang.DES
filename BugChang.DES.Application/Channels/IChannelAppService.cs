using System;
using System.Collections.Generic;
using System.Text;
using BugChang.DES.Application.Channels.Dtos;

namespace BugChang.DES.Application.Channels
{
    public interface IChannelAppService
    {
        List<ChannelListDto> GetChannels();
    }
}
