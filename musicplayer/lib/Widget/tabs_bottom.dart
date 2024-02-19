import 'package:flutter/material.dart';
import 'package:musicplayer/widget/text_icon.dart';
import 'package:provider/provider.dart';

class BottomTabs extends StatelessWidget {
  final int currentIndex;
  final ValueChanged<int> tapCallback;
  final bool showFloatPlayer;

  BottomTabs(this.currentIndex, this.tapCallback, this.showFloatPlayer);

  @override
  Widget build(BuildContext context) {
    return showFloatPlayer
        ? _buildBottomAppBar(context)
        : _buildBottomNavigationBar(context);
  }

  _buildBottomAppBar(BuildContext context) {
    return BottomAppBar(
      color: Color(0xffffffff),
      shape: CircularNotchedRectangle(),
      notchMargin: 4.0,
      child: Row(
        mainAxisSize: MainAxisSize.max,
        mainAxisAlignment: MainAxisAlignment.spaceAround,
        children: <Widget>[
          TextIcon(
            icon: Icons.whatshot,
            title: '发现',
            selected: currentIndex == 0,
            onPressed: () => tapCallback(0),
          ),
          TextIcon(
            icon: Icons.library_music,
            title: '歌单',
            selected: currentIndex == 1,
            onPressed: () => tapCallback(1),
          ),
          SizedBox(width: 70.0),
          TextIcon(
            icon: Icons.movie,
            title: 'MV',
            selected: currentIndex == 2,
            onPressed: () => tapCallback(2),
          ),
          TextIcon(
            icon: Icons.favorite,
            title: '收藏',
            selected: currentIndex == 3,
            onPressed: () => tapCallback(3),
          ),
        ],
      ),
    );
  }

  _buildBottomNavigationBar(BuildContext context) {
    return BottomNavigationBar(
      currentIndex: currentIndex,
      onTap: tapCallback,
      type: BottomNavigationBarType.fixed,
      // fixedColor: Provider.of<ColorStyleProvider>(context).getCurrentColor(),
      items: [
        BottomNavigationBarItem(
          icon: Icon(Icons.whatshot),
          label: '发现',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.library_music),
          label: '歌单',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.movie),
          label: 'MV',
        ),
        BottomNavigationBarItem(
          icon: Icon(Icons.favorite),
          label: '收藏',
        ),
      ],
    );
  }
}
