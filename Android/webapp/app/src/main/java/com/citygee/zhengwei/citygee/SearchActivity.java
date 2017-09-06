package com.citygee.zhengwei.citygee;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v7.app.ActionBarActivity;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;

import com.citygee.zhengwei.citygee.Utility.BasicUtility;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;


public class SearchActivity extends ActionBarActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_search);
        DisplaySavedKeyWords();


        //handler the search do click
        findViewById(R.id.action_do_search).setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                //save the key word to set
                String keyWord=((EditText)SearchActivity.this.findViewById(R.id.text_box_search)).getText().toString();
                SharedPreferences settings = getSharedPreferences(getString(R.string.pref_name_default), 0);
                SharedPreferences.Editor editor = settings.edit();
                Set<String> s = new HashSet<String>(settings.getStringSet(getString(R.string.pref_key_search_key), new HashSet<String>()));
                s.add(keyWord);
                editor.putStringSet(getString(R.string.pref_key_search_key),s);
                editor.commit();
                //go search
                BasicUtility.gotoNormalActivity(v.getContext(),getString(R.string.url_web_search)+keyWord);
            }
        });


    }

    @Override
    protected void onResume() {
        super.onResume();
        DisplaySavedKeyWords();
    }

    private void DisplaySavedKeyWords() {
        //display the saved key words
        SharedPreferences settings = getSharedPreferences(getString(R.string.pref_name_default), 0);
        SharedPreferences.Editor editor = settings.edit();
        Set<String> s = new HashSet<String>(settings.getStringSet(getString(R.string.pref_key_search_key), new HashSet<String>()));
        List<String> wordsToDisplay=new ArrayList<String>(Arrays.<String>asList((String[]) s.toArray(new String[s.size()])));

        ArrayAdapter<String> searchAdepter = new ArrayAdapter<String>(
                this,
                R.layout.text_search_keyworkd,
                R.id.text_keyword,
                wordsToDisplay
        );
        ListView listView=(ListView)findViewById(R.id.list_search_keywords);
        listView.setAdapter(searchAdepter);
        listView.setOnItemClickListener(new AdapterView.OnItemClickListener(){

            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                //Toast.makeText(getActivity(),((TextView)view).getText(),Toast.LENGTH_SHORT ).show();
                BasicUtility.gotoNormalActivity(view.getContext(), getString(R.string.url_web_search) + ((TextView) view).getText());
            }
        });
    }

}
