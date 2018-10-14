using Discord.Commands;

namespace DNetDebug.Results
{
    public class CommonResult : RuntimeResult
    {
        public CommonResultType CommonResultType { get; }

        public CommonResult(CommandError? error, string reason) : base(error, reason)
        {
        }

        public CommonResult(CommonResultType commonResultType, CommandError? error, string reason) : base(error, reason)
            => CommonResultType = commonResultType;

        public static CommonResult FromError(string reason)
            => new CommonResult(CommonResultType.Error, CommandError.Unsuccessful, reason);
        public static CommonResult FromSuccess(string reason = null)
            => new CommonResult(CommonResultType.Success, null, reason);
        public static CommonResult FromWarning(string reason)
            => new CommonResult(CommonResultType.Warning, null, reason);
        public static CommonResult FromInformation(string reason)
            => new CommonResult(CommonResultType.Information, null, reason);
    }
}