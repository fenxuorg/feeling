package com.example.jipe.feeling;

import android.util.Log;

import com.google.gson.Gson;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.HashMap;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.MediaType;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;

public class UserDataModel {
    private static final int DEFAULT_INIT_SIZE = 10;
    private static final int WARNING_BAR = 30;
    private static final String emailTemplate = "{\"Messages\":[{\"From\":{\"Email\":\"feelinghack@outlook.com\",\"Name\":\"FeelingTeam\"},\"To\":[{\"Email\":\"jipe@microsoft.com\",\"Name\":\"Test\"}],\"TemplateID\":492156,\"TemplateLanguage\":true,\"Subject\":\"Warning!AbnormalHeartRatse\",\"Variables\":{\"img_src\":\"https://steemitimages.com/0x0/http://ipfs.io/ipfs/QmTQo4cxDZ5MoszQAK93JyhFedeMuj7j4x5P7tQnvRi4A5\"}}]}";

    private int capacity = DEFAULT_INIT_SIZE;
    private RequestProxy storageServerProxy = new RequestProxy();
    private RequestProxy emailServerProxy = new RequestProxy("eda3492958de0eefafe0a2d1365c5522", "95bda5a6a99a0a9ca3e6d1ffa30dde5c");
    private Gson gson = new Gson();
    private int warning_count = 0;
    private boolean sending = false;

    public String user_id;
    public String started_at;
    public String ended_at;
    public ArrayList<Integer> angles;

    public UserDataModel(String userId){
        this(userId, DEFAULT_INIT_SIZE);
    }

    public UserDataModel(String userId, int capacity) {
        this.user_id = userId;
        this.capacity = capacity;
        this.started_at = GetTime();
        this.angles = new HashMap<String, Integer>();
    }

    public void add(int angle) {
        try {
            this.angles.add(angle);
            Log.i("Info:", "add: user data: <" + angle + ">, angle.length:" + angles.size());

            if (angles.size() == capacity) {
                sendMsgToStorageServer();
            }
        }
        catch(Exception e) {
            Log.e("Error", e.toString());
        }
    }

    @Override
    public String toString(){
        this.ended_at = GetTime();
        return String.format("{\"user_id\":\"%s\",\"started_at\":\"%s\",\"ended_at\":\"%s\",\"postures\":%s}", this.user_id, this.started_at, this.ended_at, gson.toJson(this.angles));
    }

    private String GetTime(){
        Date date = new Date();
        SimpleDateFormat formatter = new SimpleDateFormat("yyyyMMdd_HHmmss_SSS");
        return formatter.format(date);
    }

    private void sendMsgToStorageServer(){
        String json = this.toString();
        this.angles.clear();
        Log.i("Request", "Send json to Storage server: "+ json);
        storageServerProxy.SendRequest(
                new Request.Builder()
                        .post(RequestBody.create(MediaType.parse("application/json"), json))
                        .url("https://hackathon-student-posture.azurewebsites.net/api/v1/Posture")
                        .build(),
                new Callback() {
                    @Override
                    public void onFailure(Call call, IOException e) {
                        e.printStackTrace();
                    }

                    @Override
                    public void onResponse(Call call, Response response) throws IOException {
                        if (!response.isSuccessful()) {
                            throw new IOException("Server Error: " + response);
                        }

                        Log.i(" Server Request", "onResponse: " + response.body().string());
                    }
                }
        );
    }

    private void sendMsgToEmailService(){
        Log.i("Request", "Send warning to Email service: ");
        emailServerProxy.SendRequest(
                new Request.Builder()
                        .post(RequestBody.create(MediaType.parse("application/json"), emailTemplate))
                        .url("https://api.mailjet.com/v3.1/send")
                        .build(),
                new Callback() {
                    @Override
                    public void onFailure(Call call, IOException e) {
                        e.printStackTrace();
                    }

                    @Override
                    public void onResponse(Call call, Response response) throws IOException {
                        if (!response.isSuccessful()) {
                            throw new IOException("Server Error: " + response);
                        }

                        Log.i(" Server Request", "Email send: onResponse: " + response.body().string());
                    }
                }
        );
    }
}
