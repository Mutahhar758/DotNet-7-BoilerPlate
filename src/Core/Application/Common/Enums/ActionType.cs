using System.ComponentModel;
namespace Demo.WebApi.Application.Common.Enums;

public enum ActionType
{
    Invalid,

    [Description("View")]
    View = 100,

    [Description("Edit")]
    Edit = 200,

    [Description("Delete")]
    Delete = 300,
}
