using MongoDB.Driver;

namespace TravelAgencyManagementSystem.Forms;

internal static class UiOperation
{
    public static async Task<bool> RunAsync(Control owner, Func<Task> work)
    {
        if (owner.IsDisposed || !owner.Enabled)
            return false;
        owner.Enabled = false;
        owner.UseWaitCursor = true;
        try
        {
            await work();
            return true;
        }
        catch (Exception error)
        {
            System.Diagnostics.Trace.TraceError("Agency operation failed ({0}).", error.GetType().Name);
            var message = error switch
            {
                MongoWriteException { WriteError.Category: ServerErrorCategory.DuplicateKey } =>
                    "This username or email already exists.",
                TimeoutException or MongoConnectionException =>
                    "Check the local MongoDB service and retry. An interrupted save may finish during recovery.",
                ArgumentException or InvalidOperationException => error.Message,
                _ => "The operation failed. Retry or contact your administrator."
            };
            if (!owner.IsDisposed)
                MessageBox.Show(owner, message, "Travel Agency", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        finally
        {
            if (!owner.IsDisposed)
            {
                owner.UseWaitCursor = false;
                owner.Enabled = true;
            }
        }
    }
}
