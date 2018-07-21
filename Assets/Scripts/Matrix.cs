using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Matrix<T>
{
    class Bloc
    {
        public Bloc(int _x, int _y, T _value)
        {
            x = _x; y = _y; value = _value;
        }
        public int x;
        public int y;
        public T value;
    }

    T m_defaultValue;
    List<Bloc> m_values = new List<Bloc>();

    public Matrix(T defaultValue = default(T))
    {
        m_defaultValue = defaultValue;
    }

    public T get(int x, int y)
    {
        foreach (var v in m_values)
            if (v.x == x && v.y == y)
                return v.value;
        return m_defaultValue;
    }

    public void set(int x, int y, T value)
    {
        foreach (var v in m_values)
            if (v.x == x && v.y == y)
            {
                v.value = value;
                return;
            }
        m_values.Add(new Bloc(x, y, value));
    }

    public bool isSet(int x, int y)
    {
        foreach (var v in m_values)
            if (v.x == x && v.y == y)
                return true;
        return false;
    }

    public void reset(int x, int y)
    {
        m_values.RemoveAll(v => { return v.x == x && v.y == y; });
    }

    public void clear()
    {
        m_values.Clear();
    }

    public void setDefaultValue(T value)
    {
        m_defaultValue = value;
    }
}
