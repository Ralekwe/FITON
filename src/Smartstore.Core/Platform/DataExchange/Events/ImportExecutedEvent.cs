﻿namespace Smartstore.Core.DataExchange.Import.Events
{
    public class ImportExecutedEvent
    {
        public ImportExecutedEvent(ImportExecuteContext context)
        {
            Guard.NotNull(context, nameof(context));

            Context = context;
        }

        public ImportExecuteContext Context { get; private set; }
    }
}