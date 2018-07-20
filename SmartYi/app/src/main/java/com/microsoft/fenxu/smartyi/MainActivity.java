package com.microsoft.fenxu.smartyi;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

import com.inuker.bluetooth.library.BluetoothClient;
import com.inuker.bluetooth.library.beacon.Beacon;
import com.inuker.bluetooth.library.connect.options.BleConnectOptions;
import com.inuker.bluetooth.library.connect.response.BleConnectResponse;
import com.inuker.bluetooth.library.model.BleGattProfile;
import com.inuker.bluetooth.library.search.SearchRequest;
import com.inuker.bluetooth.library.search.SearchResult;
import com.inuker.bluetooth.library.search.response.SearchResponse;
import com.inuker.bluetooth.library.utils.BluetoothLog;

public class MainActivity extends AppCompatActivity {

    public Button BtnSearch;
    public Button BtnConnect;
    public BluetoothClient mClient;

    public boolean findMyBoard = false;
    public String MyMac = "98:D3:71:F5:C5:93";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        initView();
    }

    public void initView() {
        BtnSearch = (Button) findViewById(R.id.BtnSearch);
        BtnConnect = (Button) findViewById(R.id.BtnConnect);
        mClient = new BluetoothClient(MainActivity.this );
    }

    public void GetConnectStatus(View view)
    {

    }

    public void Connect(View view)
    {
        switch(view.getId()) {
            case R.id.BtnConnect:
                Toast.makeText(MainActivity.this, "Connect", Toast.LENGTH_LONG).show();
                ConnectBluetooth();
                break;
            default:
                Toast.makeText(MainActivity.this, "haha in connect", Toast.LENGTH_LONG).show();
                break;
        }
    }

    public void Search(View view)
    {
        switch(view.getId()) {
            case R.id.BtnSearch:
                Toast.makeText(MainActivity.this, "Search", Toast.LENGTH_LONG).show();
                SearchBluetooth();
                break;
            default:
                Toast.makeText(MainActivity.this, "haha in search", Toast.LENGTH_LONG).show();
                break;
        }
    }

    private void SearchBluetooth()
    {
        mClient.openBluetooth();

        Toast.makeText(MainActivity.this, "start-1", Toast.LENGTH_LONG).show();
        SearchRequest request = new SearchRequest.Builder()
                .searchBluetoothLeDevice(3000, 3)   // 先扫BLE设备3次，每次3s
                .searchBluetoothClassicDevice(5000) // 再扫经典蓝牙5s
                .searchBluetoothLeDevice(2000)      // 再扫BLE设备2s
                .build();
        Toast.makeText(MainActivity.this, "start-2", Toast.LENGTH_LONG).show();

        mClient.search(request, new SearchResponse() {
            @Override
            public void onSearchStarted() {
                Toast.makeText(MainActivity.this, "start searching", Toast.LENGTH_LONG).show();
            }

            @Override
            public void onDeviceFounded(SearchResult device) {
                Beacon beacon = new Beacon(device.scanRecord);
                BluetoothLog.v(String.format("beacon for %s\n%s", device.getAddress(), beacon.toString()));
                Toast.makeText(MainActivity.this, "device address:" + device.getAddress(), Toast.LENGTH_LONG).show();
                if (device.getAddress().equals(MyMac))
                {
                    Toast.makeText(MainActivity.this, "Founded:" + device.getAddress(), Toast.LENGTH_LONG).show();
                    findMyBoard = true;
                    mClient.stopSearch();
                }
            }

            @Override
            public void onSearchStopped() {
                if (findMyBoard) {
                    Toast.makeText(MainActivity.this, "find my board and stop searching", Toast.LENGTH_LONG).show();
                } else {
                    Toast.makeText(MainActivity.this, "stop searching", Toast.LENGTH_LONG).show();
                }
            }

            @Override
            public void onSearchCanceled() {
                if (findMyBoard) {
                    Toast.makeText(MainActivity.this, "find my board and cancel searching", Toast.LENGTH_LONG).show();
                }
                else {
                    Toast.makeText(MainActivity.this, "cancel searching", Toast.LENGTH_LONG).show();
                }
            }
        });

        // mClient.stopSearch();
        // mClient.closeBluetooth();
        Toast.makeText(MainActivity.this, "end", Toast.LENGTH_LONG).show();
    }

    private void ConnectBluetooth()
    {
        if (!findMyBoard)
        {
            Toast.makeText(MainActivity.this, "Can't find my blooth, stop connecting.", Toast.LENGTH_LONG).show();
            return;
        }

        BleConnectOptions options = new BleConnectOptions.Builder()
                .setConnectRetry(3)   // 连接如果失败重试3次
                .setConnectTimeout(30000)   // 连接超时30s
                .setServiceDiscoverRetry(3)  // 发现服务如果失败重试3次
                .setServiceDiscoverTimeout(20000)  // 发现服务超时20s
                .build();

        mClient.connect(MyMac, options, new BleConnectResponse() {
            @Override
            public void onResponse(int code, BleGattProfile data) {
                Toast.makeText(MainActivity.this, "Connect to my board.", Toast.LENGTH_LONG).show();
            }
        });
    }
}
