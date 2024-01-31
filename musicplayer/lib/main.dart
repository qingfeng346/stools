import 'package:audioplayers/audioplayers.dart';
import 'package:flutter/material.dart';
import 'package:musicplayer/manager/PlayerMgr.dart';
import 'package:musicplayer/util/HttpUtil.dart';
import 'package:musicplayer/widget/setting.dart';

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Flutter Demo',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      home: const MyHomePage(title: 'Music Player'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});
  final String title;
  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  int _currentTab = 0;
  int _currentDrawer = 0;
  List<Widget> tabWidgets = [
    setting(),
    setting(),
    setting()
  ];
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        title: Text(widget.title),
      ),
      body: Center(
        child: Container(
          alignment: Alignment.topCenter,
          child: SingleChildScrollView,
          children: [
            Expanded(child: tabWidgets.elementAt(_currentTab))
          ],
        ),
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: null,
        tooltip: 'Increment',
        child: const Icon(Icons.add),
      ), // This trailing comma makes auto-formatting nicer for build methods.
      bottomNavigationBar: createTabBar(),
      drawer: createDrawer(),
    );
  }

  void onTabChanged(int index) {
    print("onclick : ${index}");
    setState(() {
      this._currentTab = index;
    });
  }
  void onDrawerChanged(int index) {
    Test();
    setState(() {
      this._currentDrawer = index;
    });
  }
  void Test() async {
    // var res = await HttpUtil.httpGet("http://datools.diandian.info:7070/client/#/home/build");
    // print(res.statusCode);
    // print(res.data);
    var res = await HttpUtil.httpPost("http://datools.diandian.info:7070/execute", {'userId': "linyuan.yang", 'code': "requestServerConfig"});
    print(res.statusCode);
    print(res.data);
  }
  Widget createTabBar() {
    return new BottomNavigationBar(
      currentIndex: this._currentTab,
      onTap: this.onTabChanged,
      items: [
        BottomNavigationBarItem(icon: Icon(Icons.home), label: "主页"),
        BottomNavigationBarItem(icon: Icon(Icons.library_music), label: "音乐"),
        BottomNavigationBarItem(icon: Icon(Icons.settings), label: "设置"),
      ],
    );
  }
  Drawer createDrawer() {
    return new Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: [
          DrawerHeader(
              child: Text("111")
          ),
          ListTile(
            title: Text("Home1"),
            selected: _currentDrawer == 0,
            onTap: () {
              this.onDrawerChanged(0);
              Navigator.pop(context);
            },
          ),
          ListTile(
            title: Text("Home2"),
            selected: _currentDrawer == 1,
            onTap: () {
              this.onDrawerChanged(1);
              Navigator.pop(context);
            },
          )
        ],
      ),
    );
  }
}
