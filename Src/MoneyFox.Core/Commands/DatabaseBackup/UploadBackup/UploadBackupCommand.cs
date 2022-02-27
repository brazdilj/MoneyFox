﻿namespace MoneyFox.Core.Commands.DatabaseBackup.UploadBackup
{
    using Interfaces;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class UploadBackupCommand : IRequest
    {
        public class Handler : IRequestHandler<UploadBackupCommand, Unit>
        {
            private readonly IBackupService backupService;

            public Handler(IBackupService backupService)
            {
                this.backupService = backupService;
            }

            public async Task<Unit> Handle(UploadBackupCommand request, CancellationToken cancellationToken)
            {
                await backupService.UploadBackupAsync();
                return Unit.Value;
            }
        }
    }
}