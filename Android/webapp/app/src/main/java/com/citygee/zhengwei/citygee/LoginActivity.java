package com.citygee.zhengwei.citygee;

import android.animation.Animator;
import android.animation.AnimatorListenerAdapter;
import android.annotation.TargetApi;
import android.app.Activity;
import android.content.Intent;

import android.content.SharedPreferences;
import android.os.Build;
import android.os.Bundle;
import android.text.TextUtils;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.inputmethod.EditorInfo;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.widget.AutoCompleteTextView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.citygee.zhengwei.citygee.Utility.BasicUtility;
import com.citygee.zhengwei.citygee.webclients.LoginWebClient;

import org.apache.http.util.EncodingUtils;


/**
 * A login screen that offers login via email/password.
 */
public class LoginActivity extends Activity implements LoginWebClient.OnLogInFinishedListener{

    /**
     * A dummy authentication store containing known user names and passwords.
     * TODO: remove after connecting to a real authentication system.
     */
    private static final String[] DUMMY_CREDENTIALS = new String[]{
            "foo@example.com:hello", "bar@example.com:world"
    };
    /**
     * Keep track of the login task to ensure we can cancel it if requested.
     */
    //private UserLoginTask mAuthTask = null;

    // UI references.
    private AutoCompleteTextView mEmailView;
    private EditText mPasswordView;
    private View mProgressView;
    private View mLoginFormView;
    private WebView mWebView;
    private LoginWebClient mLoginWebViewClient;
    private boolean mIsLoggingIn;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        BasicUtility.setDefaultSettingsForTheFirstTime(this);
        setContentView(R.layout.activity_login);
        if (BasicUtility.getLoggedInPreference(this)==true){
            BasicUtility.gotoMainActivity(this);
            return;
        }
        else{
            BasicUtility.setLogIn(this,false);
        }

        // Set up the login form.
        mEmailView = (AutoCompleteTextView) findViewById(R.id.email);
        //populateAutoComplete();

        //set up user event handlers
        mPasswordView = (EditText) findViewById(R.id.password);
        mPasswordView.setOnEditorActionListener(new TextView.OnEditorActionListener() {

            @Override
            public boolean onEditorAction(TextView textView, int id, KeyEvent keyEvent) {
                //in the password editor box if user hit enter
                if (id == R.id.login || id == EditorInfo.IME_NULL) {
                    attemptLogin();
                    return true;
                }
                return false;
            }
        });
        //set up click event
        //user click logging event
        Button mEmailSignInButton = (Button) findViewById(R.id.email_sign_in_button);
        mEmailSignInButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                attemptLogin();
            }
        });

        //user click register
        Button registerButton = (Button) findViewById(R.id.register_new_button);
        registerButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                BasicUtility.gotoRegisterActivity(view.getContext());
            }
        });
        //user click guest sign in
        Button guestButton = (Button) findViewById(R.id.guest_in_button);
        guestButton.setOnClickListener(new OnClickListener() {
            @Override
            public void onClick(View view) {
                BasicUtility.gotoMainActivity(view.getContext());
            }
        });

        //see if we have a previous logged user name
        SharedPreferences settings = getSharedPreferences(getString(R.string.pref_name_default), 0);
        if(settings.contains(getString(R.string.pref_key_email))==true){
            mEmailView.setText(settings.getString(getString(R.string.pref_key_email),""));
        }



        mLoginFormView = findViewById(R.id.login_form);
        mProgressView = findViewById(R.id.login_progress);
        mWebView=(WebView)findViewById(R.id.web_view);
        mLoginWebViewClient=new LoginWebClient();
        //set up webview settings and clients
        //enable js
        WebSettings webSettings = mWebView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        //enable geo location
        webSettings.setGeolocationEnabled(true);
        //set custom agent
        webSettings.setUserAgentString(getString(R.string.user_agent_string));
        mLoginWebViewClient.setOnLogInFinishedListener(this);
        mWebView.setWebViewClient(mLoginWebViewClient);
        mIsLoggingIn=false;


    }

    private void populateAutoComplete() {
        //getLoaderManager().initLoader(0, null, this);
    }


    /**
     * Attempts to sign in or register the account specified by the login form.
     * If there are form errors (invalid email, missing fields, etc.), the
     * errors are presented and no actual login attempt is made.
     */
    public void attemptLogin() {
        if (mIsLoggingIn==true) {
            return;
        }

        // Reset errors.
        mEmailView.setError(null);
        mPasswordView.setError(null);
        //save the user name
        SharedPreferences settings = getSharedPreferences(getString(R.string.pref_name_default), 0);
        SharedPreferences.Editor editor = settings.edit();
        editor.putString(getString(R.string.pref_key_email),mEmailView.getText().toString());
        editor.commit();


        // Store values at the time of the login attempt.
        String email = mEmailView.getText().toString();
        String password = mPasswordView.getText().toString();

        boolean cancel = false;
        View focusView = null;


        // Check for a valid password, if the user entered one.
        if (!TextUtils.isEmpty(password) && !isPasswordValid(password)) {
            mPasswordView.setError(getString(R.string.error_invalid_password));
            focusView = mPasswordView;
            cancel = true;
        }

        // Check for a valid email address.
        if (TextUtils.isEmpty(email)) {
            mEmailView.setError(getString(R.string.error_field_required));
            focusView = mEmailView;
            cancel = true;
        } else if (!isEmailValid(email)) {
            mEmailView.setError(getString(R.string.error_invalid_email));
            focusView = mEmailView;
            cancel = true;
        }

        if (cancel) {
            // There was an error; don't attempt login and focus the first
            // form field with an error.
            focusView.requestFocus();
        } else {
            // Show a progress spinner, and kick off a background task to
            // perform the user login attempt.
            showProgress(true);
            mIsLoggingIn=true;
            //post the log in data to the url
            String postData="UserName="+email+"&Password="+password;
            mWebView.postUrl(getString(R.string.url_login), EncodingUtils.getBytes(postData,"BASE64"));
        }
    }



    private boolean isEmailValid(String email) {
        //TODO: Replace this with your own logic
        return email.contains("@");
    }

    private boolean isPasswordValid(String password) {
        //TODO: Replace this with your own logic
        return password.length() > 4;
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

            mLoginFormView.setVisibility(show ? View.GONE : View.VISIBLE);
            mLoginFormView.animate().setDuration(shortAnimTime).alpha(
                    show ? 0 : 1).setListener(new AnimatorListenerAdapter() {
                @Override
                public void onAnimationEnd(Animator animation) {
                    mLoginFormView.setVisibility(show ? View.GONE : View.VISIBLE);
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
            mLoginFormView.setVisibility(show ? View.GONE : View.VISIBLE);
        }
    }

    @Override
    public void OnLogInFinished(boolean isLoggedIn) {
        mIsLoggingIn = false;
        showProgress(false);

        if (isLoggedIn) {
            //save logged in state
            BasicUtility.setLogIn(this,true);
            Toast.makeText(this,"登录成功", Toast.LENGTH_SHORT).show();
            //if got here by in other activity and click on a require-auth link then redirect here
            //go to previous activity tell it to refresh
            //if user clicked back the result will not be ok the previous activity will finish
            if(getIntent().hasExtra(getString(R.string.intent_login_redirect))){
                Intent data2 = new Intent();
                if (getParent() == null) {
                    setResult(Activity.RESULT_OK, data2);
                } else {
                    getParent().setResult(Activity.RESULT_OK, data2);
                }
                finish();
            }
            else{
                //the first screen then go to main
                BasicUtility.gotoMainActivity(this);
            }
        } else {
            Toast.makeText(this,"登录失败", Toast.LENGTH_SHORT).show();
            //todo change the log in error message
            mPasswordView.setError(getString(R.string.error_incorrect_password));
            mPasswordView.requestFocus();
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if(requestCode==BasicUtility.START_REGISTER){
            //if logged  do the below code, else do nothing
            if(BasicUtility.getLoggedInPreference(this)){
                //if got here by in other activity and click on a require-auth link then redirect here
                //go to previous activity tell it to refresh
                //if user clicked back the result will not be ok the previous activity will finish
                if(getIntent().hasExtra(getString(R.string.intent_login_redirect))){
                    Intent data2 = new Intent();
                    if (getParent() == null) {
                        setResult(Activity.RESULT_OK, data2);
                    } else {
                        getParent().setResult(Activity.RESULT_OK, data2);
                    }
                    finish();
                }
                else{
                    BasicUtility.gotoMainActivity(this);
                }
            }
        }
    }

    /*
    @Override
    public Loader<Cursor> onCreateLoader(int i, Bundle bundle) {
        return new CursorLoader(this,
                // Retrieve data rows for the device user's 'profile' contact.
                Uri.withAppendedPath(ContactsContract.Profile.CONTENT_URI,
                        ContactsContract.Contacts.Data.CONTENT_DIRECTORY), ProfileQuery.PROJECTION,

                // Select only email addresses.
                ContactsContract.Contacts.Data.MIMETYPE +
                        " = ?", new String[]{ContactsContract.CommonDataKinds.Email
                .CONTENT_ITEM_TYPE},

                // Show primary email addresses first. Note that there won't be
                // a primary email address if the user hasn't specified one.
                ContactsContract.Contacts.Data.IS_PRIMARY + " DESC");
    }
    */


    /*
    @Override
    public void onLoadFinished(Loader<Cursor> cursorLoader, Cursor cursor) {
        List<String> emails = new ArrayList<String>();
        cursor.moveToFirst();
        while (!cursor.isAfterLast()) {
            emails.add(cursor.getString(ProfileQuery.ADDRESS));
            cursor.moveToNext();
        }

        addEmailsToAutoComplete(emails);
    }

    @Override
    public void onLoaderReset(Loader<Cursor> cursorLoader) {

    }

    private interface ProfileQuery {
        String[] PROJECTION = {
                ContactsContract.CommonDataKinds.Email.ADDRESS,
                ContactsContract.CommonDataKinds.Email.IS_PRIMARY,
        };

        int ADDRESS = 0;
        int IS_PRIMARY = 1;
    }


    private void addEmailsToAutoComplete(List<String> emailAddressCollection) {
        //Create adapter to tell the AutoCompleteTextView what to show in its dropdown list.
        ArrayAdapter<String> adapter =
                new ArrayAdapter<String>(LoginActivity.this,
                        android.R.layout.simple_dropdown_item_1line, emailAddressCollection);

        mEmailView.setAdapter(adapter);
    }
    */

    /**
     * Represents an asynchronous login/registration task used to authenticate
     * the user.
     */
    /*
    public class UserLoginTask extends AsyncTask<Void, Void, Boolean> {

        private final String mEmail;
        private final String mPassword;

        UserLoginTask(String email, String password) {
            mEmail = email;
            mPassword = password;
        }

        @Override
        protected Boolean doInBackground(Void... params) {
            // attempt authentication against a network service.

            try {
                // Simulate network access.
                Thread.sleep(2000);
            } catch (InterruptedException e) {
                return false;
            }

            for (String credential : DUMMY_CREDENTIALS) {
                String[] pieces = credential.split(":");
                if (pieces[0].equals(mEmail)) {
                    // Account exists, return true if the password matches.
                    return pieces[1].equals(mPassword);
                }
            }

            // register the new account here.
            return true;
        }

        @Override
        protected void onPostExecute(final Boolean success) {
            mAuthTask = null;
            showProgress(false);

            if (success) {
                finish();
            } else {
                mPasswordView.setError(getString(R.string.error_incorrect_password));
                mPasswordView.requestFocus();
            }
        }

        @Override
        protected void onCancelled() {
            mAuthTask = null;
            showProgress(false);
        }
    }
    */
}



