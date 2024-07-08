

import 'package:path_provider/path_provider.dart';
import 'package:sqlite3/sqlite3.dart';

class MusicMgr {
  static MusicMgr? _instance;
  static get Instance {
    if (_instance == null) {
      _instance = new MusicMgr();
    }
    return _instance;
  }
  MusicMgr() {
    // getApplicationDocumentsDirectory()
    // sqlite3.open(Path)
  }
}