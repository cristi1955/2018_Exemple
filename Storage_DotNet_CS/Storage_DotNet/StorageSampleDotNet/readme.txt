To use this sample, edit app.config and change the storage account connection string.
Replace MYSTORAGEACCOUNT with the name of your Windows Azure storage account.
Replace MYSTORAGEKEY with a key for your Windows Azure storage account.
Storage account projects and keys are managed in the Windows Azure portal (http://windows.azure.com).

<add name="Storage" connectionString="DefaultEndpointsProtocol=http;AccountName=MYSTORAGEACCOUNT;AccountKey=MYSTORAGEKEY" />

Warning: this program creates, modifies, and deletes data. Only use it with a test storage area.