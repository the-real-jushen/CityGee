package com.citygee.zhengwei.citygee.webclients;

import android.content.Context;
import android.util.AttributeSet;
import android.webkit.WebView;

/**
 * Created by zhengwei on 2015/7/14.
 */
public class ObservableWebView extends WebView
{
    public int mScrollDistanceY;

    private OnScrollChangedHandler mOnScrollChangedHandler;

    public ObservableWebView(final Context context)
    {
        super(context);
    }

    public ObservableWebView(final Context context, final AttributeSet attrs)
    {
        super(context, attrs);
    }

    public ObservableWebView(final Context context, final AttributeSet attrs, final int defStyle)
    {
        super(context, attrs, defStyle);
    }

    @Override
    protected void onScrollChanged(final int l, final int t, final int oldl, final int oldt)
    {
        super.onScrollChanged(l, t, oldl, oldt);
        int dis = t - oldt;
        if(dis*mScrollDistanceY>0){
            //scroll same direction
            mScrollDistanceY+=dis;
        }
        else
        {
            mScrollDistanceY=dis;
        }
        if(mOnScrollChangedHandler != null) mOnScrollChangedHandler.onScroll(l, t,mScrollDistanceY);
    }

    public OnScrollChangedHandler getOnScrollChangedCallback()
    {
        return mOnScrollChangedHandler;
    }

    public void setOnScrollChangedHandler(final OnScrollChangedHandler onScrollChangedHandler)
    {
        mOnScrollChangedHandler = onScrollChangedHandler;
    }

    /**
     * Impliment in the activity/fragment/view that you want to listen to the webview
     */
    public static interface OnScrollChangedHandler
    {
        public void onScroll(int l, int t,int distanceY);
    }
}
