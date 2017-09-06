package com.citygee.zhengwei.citygee;

import android.animation.Animator;
import android.animation.AnimatorListenerAdapter;
import android.annotation.TargetApi;
import android.os.Build;
import android.support.v7.app.ActionBarActivity;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.citygee.zhengwei.citygee.restclient.RestClient;
import com.loopj.android.http.TextHttpResponseHandler;

import org.apache.http.Header;


public class FeedbackActivity extends ActionBarActivity {


    private View mBeedbackFormView;
    private View mProgressView;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_feedback);
        mBeedbackFormView = findViewById(R.id.feedback_form);
        mProgressView = findViewById(R.id.login_progress);
        //set client here,use anonymous class

        ////post the log in data to the url
        //set up click event
        //user click happy event
        Button mEmailSignInButton = (Button) findViewById(R.id.action_feedback_happy);
        mEmailSignInButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                String content = ((EditText) findViewById(R.id.feedback_text)).getText().toString();
                String mood =  ((Button)view).getText().toString();
                postFeedback(content+":mood:"+mood);
            }
        });

        //user click confused
        Button registerButton = (Button) findViewById(R.id.action_feedback_confused);
        registerButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                String content = ((EditText) findViewById(R.id.feedback_text)).getText().toString();
                String mood =  ((Button)view).getText().toString();
                postFeedback(content + ":mood:" + mood);
            }
        });
        //user click fucked up
        Button guestButton = (Button) findViewById(R.id.action_feedback_fuckedup);
        guestButton.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                String content = ((EditText) findViewById(R.id.feedback_text)).getText().toString();
                String mood =  ((Button)view).getText().toString();
                postFeedback(content + ":mood:" + mood);
            }
        });


    }

    private void postFeedback(String content){
        showProgress(true);
        RestClient.postFeedbackAsync(content,new FeedbackPostedHandler());
    }


    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        //getMenuInflater().inflate(R.menu.menu_feedback, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    private class FeedbackPostedHandler extends TextHttpResponseHandler {


        @Override
        public void onFailure(int i, Header[] headers, String s, Throwable throwable) {

        }

        @Override
        public void onSuccess(int i, Header[] headers, String s) {
            Toast.makeText(FeedbackActivity.this, "感谢您的意见", Toast.LENGTH_SHORT).show();
            try {
                finish();
            }
            catch (Exception e){
                //does nothing, the activity is already finished
            }
        }
    }

    /**
     * Shows the progress UI and hides the login form.
     */
    @TargetApi(Build.VERSION_CODES.HONEYCOMB_MR2)
    public void showProgress(final boolean show) {
        // On Honeycomb MR2 we have the ViewPropertyAnimator APIs, which allow
        // for very easy animations. If available, use these APIs to fade-in
        // the progress spinner.
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.HONEYCOMB_MR2) {
            int shortAnimTime = getResources().getInteger(android.R.integer.config_shortAnimTime);

            mBeedbackFormView.setVisibility(show ? View.GONE : View.VISIBLE);
            mBeedbackFormView.animate().setDuration(shortAnimTime).alpha(
                    show ? 0 : 1).setListener(new AnimatorListenerAdapter() {
                @Override
                public void onAnimationEnd(Animator animation) {
                    mBeedbackFormView.setVisibility(show ? View.GONE : View.VISIBLE);
                }
            });

            mProgressView.setVisibility(show ? View.VISIBLE : View.GONE);
            mProgressView.animate().setDuration(shortAnimTime).alpha(
                    show ? 1 : 0).setListener(new AnimatorListenerAdapter() {
                @Override
                public void onAnimationEnd(Animator animation) {
                    mProgressView.setVisibility(show ? View.VISIBLE : View.GONE);
                }
            });
        } else {
            // The ViewPropertyAnimator APIs are not available, so simply show
            // and hide the relevant UI components.
            mProgressView.setVisibility(show ? View.VISIBLE : View.GONE);
            mBeedbackFormView.setVisibility(show ? View.GONE : View.VISIBLE);
        }
    }
}
