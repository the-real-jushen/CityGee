package com.citygee.zhengwei.citygee.Utility;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.util.Log;
import android.webkit.CookieManager;
import android.webkit.WebView;

import com.citygee.zhengwei.citygee.FeedbackActivity;
import com.citygee.zhengwei.citygee.LikeActivity;
import com.citygee.zhengwei.citygee.MyApp;
import com.citygee.zhengwei.citygee.NormalActivity;
import com.citygee.zhengwei.citygee.LoginActivity;
import com.citygee.zhengwei.citygee.MainActivity;
import com.citygee.zhengwei.citygee.R;
import com.citygee.zhengwei.citygee.RegisterActivity;
import com.citygee.zhengwei.citygee.SearchActivity;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.HashSet;
import java.util.Set;

/**
 * Created by zhengwei on 2015/7/8.
 */

//set the default preference for the first time only when no preference is set before, otherwise it make no change
public class BasicUtility {
    public static final int START_REGISTER = 10;
    public static final int START_LOGIN =11 ;

    public static void setDefaultSettingsForTheFirstTime(Context context){
        SharedPreferences settings = context.getSharedPreferences(context.getString(R.string.pref_name_default), 0);
        SharedPreferences.Editor editor = settings.edit();
        //set if logged in
        if(settings.contains(context.getString(R.string.pref_key_loggedin))==false){
            editor.putBoolean(context.getString(R.string.pref_key_loggedin),false);
            editor.commit();
        }
        // Commit the edits!
        //set search hot key word
        if(settings.contains(context.getString(R.string.pref_key_search_key))==false){
            Set<String> searchKeys = new HashSet<String>();
            searchKeys.add("汉口");
            searchKeys.add("黎黄陂路");
            searchKeys.add("华中科技大学");
            searchKeys.add("约");
            editor.putStringSet(context.getString(R.string.pref_key_search_key),searchKeys);
            editor.commit();
        }

    }

    public static void setLogIn(Context context,boolean isLoggedIn){
        SharedPreferences settings = context.getSharedPreferences(context.getString(R.string.pref_name_default), 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putBoolean(context.getString(R.string.pref_key_loggedin),isLoggedIn);
        editor.commit();

        if(isLoggedIn==false){
            //force log out
            CookieManager cookieManager = CookieManager.getInstance();
            cookieManager.removeAllCookie();
            editor.putString(context.getString(R.string.pref_key_uid),null);
            editor.commit();
        }
        else
        {
            //save uid and name
            new WebApiUtility().getUserIdAsync(context,new WebApiUtility.OnGetAppUserListener() {
                @Override
                public void onGetAppUser(AppUser user) {
                    //another scope editor is different from the outer one
                    SharedPreferences settings = MyApp.jusGetContext().getSharedPreferences(MyApp.jusGetContext().getString(R.string.pref_name_default), 0);
                    SharedPreferences.Editor editor = settings.edit();
                    editor.putString(MyApp.jusGetResources().getString(R.string.pref_key_uid),user.Id);
                    editor.putString(MyApp.jusGetResources().getString(R.string.pref_key_username),user.UserName);
                    editor.commit();
                }
            });
        }

    }

    //check if iut is the first time i run this app for today
    //if so check upgrade
    public static boolean timeToUpgrade(Context context){
        SharedPreferences settings = context.getSharedPreferences(context.getString(R.string.pref_key_update_date), 0);
        SharedPreferences.Editor editor = settings.edit();
        if(settings.contains(context.getString(R.string.pref_key_update_date))==false){
            //set update today
            Calendar c = Calendar.getInstance();
            SimpleDateFormat df = new SimpleDateFormat("dd-MMM-yyyy");
            String formattedDate = df.format(c.getTime());
            editor.putString(context.getString(R.string.pref_key_loggedin), formattedDate);
            editor.commit();
            return true;
        }
        String lastUpdate=settings.getString(context.getString(R.string.pref_key_update_date), "");
        Calendar c = Calendar.getInstance();
        SimpleDateFormat df = new SimpleDateFormat("dd-MMM-yyyy");
        String formattedDate = df.format(c.getTime());
        if(formattedDate.equals(lastUpdate))
        {
            return false;
        }
        return true;
    }


    public static boolean getLoggedInPreference(Context context){
        SharedPreferences settings = context.getSharedPreferences(context.getString(R.string.pref_name_default), 0);
        return settings.getBoolean(context.getString(R.string.pref_key_loggedin),false);
    }


    public static void gotoMainActivity(Context context) {
        Intent intent = new Intent(context,MainActivity.class);
        intent.putExtra(context.getString(R.string.intent_start_url),context.getString(R.string.intent_not_url));
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        context.startActivity(intent);
    }

    public static void gotoFeedbackActivity(Context context) {
        Intent intent = new Intent(context,FeedbackActivity.class);
        context.startActivity(intent);
    }

    //call this only in log in activity
    public static void gotoRegisterActivity(Context context) {
        Intent intent = new Intent(context,RegisterActivity.class);
        intent.putExtra(context.getString(R.string.intent_start_url),context.getString(R.string.intent_not_url));
        ((Activity)context).startActivityForResult(intent, START_REGISTER);
    }

    public static void gotoLoginActivity(Context context) {
        Intent intent = new Intent(context,LoginActivity.class);
        intent.putExtra(context.getString(R.string.intent_start_url),context.getString(R.string.intent_not_url));
        intent.putExtra(context.getString(R.string.intent_login_redirect),context.getString(R.string.intent_login_redirect));
        //make sure we log out
        setLogIn(context,false);
        ((Activity)context).startActivityForResult(intent, START_LOGIN);
    }

    public static void gotoSearchActivity(Context context) {
        Intent intent = new Intent(context, SearchActivity.class);
        intent.putExtra(context.getString(R.string.intent_start_url),context.getString(R.string.intent_not_url));
        context.startActivity(intent);
    }

    public static void gotoNormalActivity(Context context, String url) {
        Intent intent = new Intent(context, NormalActivity.class);
        intent.putExtra(context.getString(R.string.intent_start_url),url);
        context.startActivity(intent);
    }

    public static void gotoLikeActivity(Context context, String url) {
        Intent intent = new Intent(context, LikeActivity.class);
        intent.putExtra(context.getString(R.string.intent_start_url),url);
        context.startActivity(intent);
    }

    public static boolean getLogInState(Context context){
        CookieManager cookieManager = CookieManager.getInstance();
        String cookie=cookieManager.getCookie(context.getString(R.string.url_web_root));
        if(cookie!=null && cookie.contains(context.getString(R.string.cookie_name_login)))
        {
            return true;
        }
        return false;
    }

    //toggle like of the house or check in
    public static void toggleLike(Context context, String id){
        ((WebView)((Activity)context).findViewById(R.id.web_view)).loadUrl("javascript: setupLike('" + id + "','Browser')");
    }


    public static String generateTitle(String title) {
        return title.replace("城迹--","");

    }
}
