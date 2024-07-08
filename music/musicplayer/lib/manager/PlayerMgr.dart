
import 'package:audioplayers/audioplayers.dart';

class PlayerMgr {
  static PlayerMgr? _instance;
  static get Instance {
    if (_instance == null) {
      _instance = new PlayerMgr();
    }
    return _instance;
  }
  AudioPlayer? audioPlayer;
  PlayerMgr() {
    this.audioPlayer = new AudioPlayer()..setReleaseMode(ReleaseMode.stop);
    // this.audioPlayer.onDurationChanged.listen((event) { })
  }
}