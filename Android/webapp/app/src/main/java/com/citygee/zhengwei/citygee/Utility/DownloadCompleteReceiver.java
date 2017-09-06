package com.citygee.zhengwei.citygee.Utility;

import android.app.DownloadManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.net.Uri;
import android.os.Environment;

import com.citygee.zhengwei.citygee.MyApp;

import java.io.File;

import static android.support.v4.app.ActivityCompat.startActivity;
/**
 * Created by zhengwei on 2015/7/11.
 */
public class DownloadCompleteReceiver extends BroadcastReceiver {
    @Override
    public void onReceive(Context context, Intent intent) {
        long id = intent.getExtras().getLong(DownloadManager.EXTRA_DOWNLOAD_ID);

        if (id ==MyApp.apkDownloadId) {
            DownloadManager dManager = (DownloadManager) MyApp.jusGetContext().getSystemService(Context.DOWNLOAD_SERVICE);
            Uri downloadFileUri = dManager.getUriForDownloadedFile(id);
            Intent installIntent = new Intent(Intent.ACTION_VIEW);
            installIntent.setDataAndType(downloadFileUri, "application/vnd.android.package-archive");
            //installIntent.setDataAndType(Uri.fromFile(new File(context.getExternalFilesDir(null), "citygee.apk")), "application/vnd.android.package-archive");
            installIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
            MyApp.jusGetContext().startActivity(installIntent);
        }
    }
}
