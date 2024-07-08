import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class TextIcon extends StatelessWidget {
  final IconData icon;
  final String title;
  final bool selected;
  final VoidCallback onPressed;
  const TextIcon({super.key, required this.title, required this.icon, required this.selected, required this.onPressed});
  @override
  Widget build(BuildContext context) {
    // Color color = selected ? Provider.of<ColorStyleProvider>(context).getCurrentColor() : Colors.black54;
    return InkWell(
        onTap: this.onPressed,
        child: Container(
            width: 66.0,
            padding: EdgeInsets.fromLTRB(4.0, 8.0, 4.0, 4.0),
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: <Widget>[
                Icon(icon, color: Colors.black54, size: 22.0),
                Text(
                  title,
                  style: TextStyle(
                      //fontSize: selected ? 14.0 : 12.0,
                      fontSize: 14.0,
                      height: 1.4,
                      color: Colors.black54),
                )
              ],
            )));
  }
}
