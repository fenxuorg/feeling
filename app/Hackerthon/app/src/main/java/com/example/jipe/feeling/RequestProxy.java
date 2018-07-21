package com.example.jipe.feeling;

import org.apache.http.client.config.RequestConfig;
import org.apache.http.impl.nio.client.CloseableHttpAsyncClient;
import org.apache.http.impl.nio.client.HttpAsyncClients;

public class RequestProxy {
    RequestConfig requestConfig = RequestConfig.custom()
            .setSocketTimeout(3000).setConnectTimeout(3000).build();
    CloseableHttpAsyncClient httpclient = HttpAsyncClients.custom()
            .setDefaultRequestConfig(requestConfig).build();

    public RequestProxy(){

    }
}
