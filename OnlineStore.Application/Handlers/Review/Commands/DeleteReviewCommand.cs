using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStore.Application.Handlers.Review.Commands
{
    public sealed record DeleteReviewCommand
    (
        int ReviewId,
        string? DeleteReason
    );
}
